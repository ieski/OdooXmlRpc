namespace Odoo.Concrete
{
    class RpcThreadPayload
    {
        public RpcModel Model { get; set; }
        public int StartIndex { get; set; }
        public int RequestSize { get; set; }
        public int ThreadCountSize { get; set; }

        public RpcThreadPayload(RpcModel model, int startIndex, int requestSize, int threadCountSize)
        {
            StartIndex = startIndex;
            Model = model;
            RequestSize = requestSize;
            ThreadCountSize = threadCountSize;
        }
    }
}
