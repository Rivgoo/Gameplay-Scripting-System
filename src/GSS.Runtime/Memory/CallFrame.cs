namespace GSS.Runtime.Memory
{
    public readonly struct CallFrame
    {
        public readonly int ReturnIP;
        public readonly int BaseRegister;

        public CallFrame(int returnIP, int baseRegister)
        {
            ReturnIP = returnIP;
            BaseRegister = baseRegister;
        }
    }
}