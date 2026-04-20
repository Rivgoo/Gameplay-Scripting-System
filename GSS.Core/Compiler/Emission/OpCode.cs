namespace GSS.Core.Compiler.Emission
{
	public enum OpCode : byte
	{
		Nop,
		Jump,
		BranchTrue,
		BranchFalse,

		LoadConst,
		LoadReg,
		StoreReg,
		LoadProp,
		StoreProp,

		PushArg,
		CallMethod,

		AddInt, SubInt, MulInt, DivInt, ModInt,
		AddFloat, SubFloat, MulFloat, DivFloat, ModFloat,
		AddString,

		EqInt, NeqInt, LtInt, GtInt, LteInt, GteInt,
		EqFloat, NeqFloat, LtFloat, GtFloat, LteFloat, GteFloat,

		NotBool,
		NegateInt, NegateFloat,
		BitNotInt, BitNotLong,

		CastIntToFloat, CastFloatToInt, CastAnyToString,

		CreateArray,
		CreateList,
		GetCollectionLength,
		LoadElement,
		StoreElement,

		Wait,
		Halt
	}
}