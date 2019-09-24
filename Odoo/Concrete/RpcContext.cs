using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Odoo.Concrete
{
    public class RpcContext
    {
        private readonly RpcConnection _rpcConnection;
        private RpcModel _rpcModel;
        private readonly string _modelName;
        private List<RpcRecord> _records;

        public List<string> FieldNames { get; set; }
        public RpcFilter RpcFilter { get; set; }

        public RpcContext(RpcConnection rpcConnection, string modelName)
        {
            _rpcConnection = rpcConnection;
            _modelName = modelName;
            RpcFilter = new RpcFilter();
            FieldNames = new List<string>();
        }

        public List<RpcRecord> GetRecords()
        {
            return _records;
        }

        /// <summary>
        /// Sonuçları XML Çevir
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            var root = new XElement(_modelName);

            foreach (var record in _records)
            {
                var element = new XElement("Item");
                foreach (var field in record.GetFields())
                {
                    element.Add(new XAttribute(field.Key, field.Value));
                }
                root.Add(element);
            }
            return root;
        }

        /// <summary>
        ///     Read parametresini True atarsan ODOO üzerinde READ işleminide yapar
        /// </summary>
        /// <param name="read"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<RpcRecord> Execute(bool read = false, int? offset = null, int? limit = null)
        {
            if (!_rpcConnection.Login())
            {
                _rpcConnection.Login();
            }

            _rpcModel = _rpcConnection.GetModel(_modelName);
            _rpcModel.AddFields(FieldNames);

            return !read ? _rpcModel.Search(RpcFilter.ToArray()) : _rpcModel.SearchAndRead(RpcFilter.ToArray(), offset, limit);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCount">Açılacak thread sayısı</param>
        /// <param name="requestSize">Tek seferde alınacak veri miktarı</param>
        /// <returns></returns>
        public List<RpcRecord> ExecuteThread(int threadCount, int requestSize)
        {
            if (!_rpcConnection.Login())
            {
                _rpcConnection.Login();
            }

            _rpcModel = _rpcConnection.GetModel(_modelName);
            _rpcModel.AddFields(FieldNames);

            threadCount = threadCount == 0 ? 1 : threadCount;

            var totalRecordCount = Count();
            _records = new List<RpcRecord>(totalRecordCount);


            //Thread başına düşecek kayıt sayısı
            var threadCountSize = Convert.ToInt32(Math.Ceiling((decimal)(totalRecordCount / threadCount))) + 1;

            //Toplam Kayıt Sayısı tek seferde alınacak request size'dan az ise tek thread aç
            if (totalRecordCount < requestSize)
            {
                threadCount = 1;
                requestSize = totalRecordCount;
                threadCountSize = requestSize;
            }
            else
            {
                requestSize = threadCountSize < requestSize ? threadCountSize : requestSize;
            }

            var threadList = new List<Thread>(threadCount);
            var startIndex = 0;

            for (var i = 0; i < threadCount; i++)
            {
                var t = new Thread((payload) =>
                {
                    var threadPayload = (RpcThreadPayload)payload;
                    var currentIndex = threadPayload.StartIndex;

                    while (true)
                    {

                        if (((currentIndex - threadPayload.StartIndex) + threadPayload.RequestSize) > threadPayload.ThreadCountSize)
                        {
                            threadPayload.RequestSize = threadPayload.ThreadCountSize - (currentIndex - threadPayload.StartIndex);
                        }

                        var records = threadPayload.Model.SearchAndRead(
                            RpcFilter.ToArray(),
                            currentIndex,
                            threadPayload.RequestSize);

                        if (records.Count == 0) break;

                        lock (_records)
                        {
                            _records.AddRange(records);
                        }

                        currentIndex += threadPayload.RequestSize;

                        if ((currentIndex - threadPayload.StartIndex) >= threadPayload.ThreadCountSize) break;

                    }
                });

                threadList.Add(t);
                t.Start(new RpcThreadPayload(_rpcModel, startIndex, requestSize, threadCountSize));

                startIndex += threadCountSize;
            }

            while (true)
            {
                var any = threadList.Any(x => x.IsAlive);
                if (!any) break;
            }

            return _records;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCount">Açılacak thread sayısı</param>
        /// <param name="requestSize">Tek seferde alınacak veri miktarı</param>
        /// <returns></returns>
        public List<RpcRecord> ExecuteAsync(int threadCount, int requestSize)
        {
            if (!_rpcConnection.Login())
            {
                _rpcConnection.Login();
            }

            _rpcModel = _rpcConnection.GetModel(_modelName);
            _rpcModel.AddFields(FieldNames);

            threadCount = threadCount == 0 ? 1 : threadCount;

            var totalRecordCount = Count();
            _records = new List<RpcRecord>(totalRecordCount);

            //Thread başına düşecek kayıt sayısı
            var threadCountSize = Convert.ToInt32(Math.Ceiling((decimal)(totalRecordCount / threadCount))) + 1;

            //Toplam Kayıt Sayısı tek seferde alınacak request size'dan az ise tek thread aç
            if (totalRecordCount < requestSize)
            {
                threadCount = 1;
                requestSize = totalRecordCount;
                threadCountSize = requestSize;
            }
            else
            {
                requestSize = threadCountSize < requestSize ? threadCountSize : requestSize;
            }

            var threadList = new Task[threadCount];
            var startIndex = 0;

            void Action(object payload)
            {
                var threadPayload = (RpcThreadPayload)payload;

                var currentIndex = threadPayload.StartIndex;

                while (true)
                {
                    if (currentIndex - threadPayload.StartIndex + threadPayload.RequestSize > threadPayload.ThreadCountSize)
                    {
                        threadPayload.RequestSize = threadPayload.ThreadCountSize - (currentIndex - threadPayload.StartIndex);
                    }

                    var records = threadPayload.Model.SearchAndRead(RpcFilter.ToArray(), currentIndex, threadPayload.RequestSize);

                    if (records.Count == 0) break;

                    lock (_records)
                    {
                        _records.AddRange(records);
                    }

                    currentIndex += threadPayload.RequestSize;

                    if (currentIndex - threadPayload.StartIndex >= threadPayload.ThreadCountSize) break;
                }
            }


            for (var i = 0; i < threadCount; i++)
            {
                var payload = new RpcThreadPayload(_rpcModel, startIndex, requestSize, threadCountSize);
                var t = new Task(Action, payload);
                t.Start();
                threadList[i] = t;
                startIndex += threadCountSize;
            }

            Task.WaitAll(threadList);

            return _records;
        }

        /// <summary>
        /// Toplam kayıt sayısı
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            if (!_rpcConnection.Login()) _rpcConnection.Login();
            var filter = RpcFilter.ToArray();
            _rpcModel = _rpcConnection.GetModel(_modelName);

            return _rpcModel.Count(filter);
        }

        public RpcContext AddField(string fieldName)
        {
            FieldNames.Add(fieldName);
            return this;
        }

        public RpcContext AddFields(List<string> fieldsName)
        {
            FieldNames.AddRange(fieldsName);
            return this;
        }
    }
}
