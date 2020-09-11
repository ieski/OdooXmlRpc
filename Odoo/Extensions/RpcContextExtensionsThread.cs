//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Odoo.Concrete;

//namespace Odoo.Entensions
//{
//    public static class RpcContextEntensionsThread
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="threadCount">Açılacak thread sayısı</param>
//        /// <param name="requestSize">Tek seferde alınacak veri miktarı</param>
//        /// <returns></returns>
//        public static List<RpcRecord> ExecuteThread(this RpcContext context, int threadCount, int requestSize)
//        {
//            if (!context.Connection.Login()) context.Connection.Login();

//            _rpcModel.AddFields(FieldNames);

//            threadCount = threadCount == 0 ? 1 : threadCount;

//            var totalRecordCount = Count();
//            _records = new List<RpcRecord>(totalRecordCount);


//            //Thread başına düşecek kayıt sayısı
//            var threadCountSize = Convert.ToInt32(Math.Ceiling((decimal)(totalRecordCount / threadCount))) + 1;

//            //Toplam Kayıt Sayısı tek seferde alınacak request size'dan az ise tek thread aç
//            if (totalRecordCount < requestSize)
//            {
//                threadCount = 1;
//                requestSize = totalRecordCount;
//                threadCountSize = requestSize;
//            }
//            else
//            {
//                requestSize = threadCountSize < requestSize ? threadCountSize : requestSize;
//            }

//            var threadList = new List<Thread>(threadCount);
//            var startIndex = 0;

//            for (var i = 0; i < threadCount; i++)
//            {
//                var t = new Thread((payload) =>
//                {
//                    var threadPayload = (RpcThreadPayload)payload;
//                    var currentIndex = threadPayload.StartIndex;

//                    while (true)
//                    {

//                        if (((currentIndex - threadPayload.StartIndex) + threadPayload.RequestSize) > threadPayload.ThreadCountSize)
//                        {
//                            threadPayload.RequestSize = threadPayload.ThreadCountSize - (currentIndex - threadPayload.StartIndex);
//                        }

//                        var records = threadPayload.Model.SearchAndRead(
//                            RpcFilter.ToArray(),
//                            currentIndex,
//                            threadPayload.RequestSize);

//                        if (records.Count == 0) break;

//                        lock (_records)
//                        {
//                            _records.AddRange(records);
//                        }

//                        currentIndex += threadPayload.RequestSize;

//                        if ((currentIndex - threadPayload.StartIndex) >= threadPayload.ThreadCountSize) break;

//                    }
//                });

//                threadList.Add(t);
//                t.Start(new RpcThreadPayload(_rpcModel, startIndex, requestSize, threadCountSize));

//                startIndex += threadCountSize;
//            }

//            while (true)
//            {
//                var any = threadList.Any(x => x.IsAlive);
//                if (!any) break;
//            }

//            return _records;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="threadCount">Açılacak thread sayısı</param>
//        /// <param name="requestSize">Tek seferde alınacak veri miktarı</param>
//        /// <returns></returns>
//        public static List<RpcRecord> ExecuteAsync(this RpcContext context, int threadCount, int requestSize)
//        {
//            if (!_rpcConnection.Login()) _rpcConnection.Login();

//            _rpcModel.AddFields(FieldNames);

//            threadCount = threadCount == 0 ? 1 : threadCount;

//            var totalRecordCount = Count();
//            _records = new List<RpcRecord>(totalRecordCount);

//            //Thread başına düşecek kayıt sayısı
//            var threadCountSize = Convert.ToInt32(Math.Ceiling((decimal)(totalRecordCount / threadCount))) + 1;

//            //Toplam Kayıt Sayısı tek seferde alınacak request size'dan az ise tek thread aç
//            if (totalRecordCount < requestSize)
//            {
//                threadCount = 1;
//                requestSize = totalRecordCount;
//                threadCountSize = requestSize;
//            }
//            else
//            {
//                requestSize = threadCountSize < requestSize ? threadCountSize : requestSize;
//            }

//            var threadList = new Task[threadCount];
//            var startIndex = 0;

//            void Action(object payload)
//            {
//                var threadPayload = (RpcThreadPayload)payload;

//                var currentIndex = threadPayload.StartIndex;

//                while (true)
//                {
//                    if (currentIndex - threadPayload.StartIndex + threadPayload.RequestSize > threadPayload.ThreadCountSize)
//                    {
//                        threadPayload.RequestSize = threadPayload.ThreadCountSize - (currentIndex - threadPayload.StartIndex);
//                    }

//                    var records = threadPayload.Model.SearchAndRead(RpcFilter.ToArray(), currentIndex, threadPayload.RequestSize);

//                    if (records.Count == 0) break;

//                    lock (_records)
//                    {
//                        _records.AddRange(records);
//                    }

//                    currentIndex += threadPayload.RequestSize;

//                    if (currentIndex - threadPayload.StartIndex >= threadPayload.ThreadCountSize) break;
//                }
//            }


//            for (var i = 0; i < threadCount; i++)
//            {
//                var payload = new RpcThreadPayload(_rpcModel, startIndex, requestSize, threadCountSize);
//                var t = new Task(Action, payload);
//                t.Start();
//                threadList[i] = t;
//                startIndex += threadCountSize;
//            }

//            Task.WaitAll(threadList);

//            return _records;
//        }
//    }
//}
