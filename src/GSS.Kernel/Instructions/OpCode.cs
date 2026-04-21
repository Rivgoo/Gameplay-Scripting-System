namespace GSS.Kernel.Instructions
{
    public enum OpCode : byte
    {
        // System
        Nop, Halt, Abort,

        // Memory & Registers
        LoadConst, LoadReg, StoreReg, MoveReg,

        // Interop & Calling
        PushArg, CallExt, CallInt, Ret, LoadProp, StoreProp,

        // Control Flow
        Jump, BranchTrue, BranchFalse,

        // Relational & Logic
        Eq, Neq, Gt, Lt, Gte, Lte,
        And, Or, Not,

        // Integer Math
        AddInt, SubInt, MulInt, DivInt, ModInt, NegInt,

        // Float Math
        AddFloat, SubFloat, MulFloat, DivFloat, ModFloat, NegFloat,

        // Vector Math
        AddVec, SubVec, MulVec,

        // Strings
        Concat,

        // Collections
        NewArr, LdLen, LdElem, StElem,

        // Casts
        ConvIntToFloat, ConvFloatToInt, ConvToStr
    }
}