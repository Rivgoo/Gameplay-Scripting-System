namespace GSS.Core.Compiler.Emission
{
	public enum OpCode : byte
	{
		Nop,
		Halt,
		Abort,
		Wait,

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
		AddDouble, SubDouble, MulDouble, DivDouble, ModDouble,
		AddLong, SubLong, MulLong, DivLong, ModLong,

		AddString, EqString, NeqString,

		EqInt, NeqInt, LtInt, GtInt, LteInt, GteInt,
		EqFloat, NeqFloat, LtFloat, GtFloat, LteFloat, GteFloat,
		EqBool, NeqBool, AndBool, OrBool, XorBool, NotBool,

		NegateInt, NegateFloat,
		BitNotInt, BitNotLong, ShlInt, ShrInt,

		CastIntToFloat, CastFloatToInt, CastAnyToString,

		CreateArray,
		CreateList,
		GetArrayLength,
		GetListCount,

		LoadElement_GssArray,
		StoreElement_GssArray,
		LoadElement_GssList,
		StoreElement_GssList,
		LoadElement_IntArray,
		StoreElement_IntArray,
		LoadElement_FloatArray,
		StoreElement_FloatArray,

		ListAdd_GssValue,
		ListRemoveAt_GssValue,
		ListClear
	}
}