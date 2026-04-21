using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Execution;
using GSS.Kernel.Instructions;
using GSS.Kernel.Primitives;
using GSS.Runtime.Exceptions;

using DivideByZeroException = GSS.Runtime.Exceptions.DivideByZeroException;
using RuntimeExecutionContext = GSS.Runtime.Memory.RuntimeExecutionContext;

namespace GSS.Runtime.Engine
{
    public static class VirtualMachine
    {
        public static ExecutionState Execute(RuntimeExecutionContext ctx, int maxSteps = 10000)
        {
            ReadOnlySpan<Instruction> code = ctx.Graph.Instructions;
            ReadOnlySpan<Variant> consts = ctx.Graph.Constants;
            ReadOnlySpan<object> meta = ctx.Graph.Metadata;

            ref int ip = ref ctx.InstructionPointer;
            ref Variant acc = ref ctx.Accumulator;
            
            int steps = 0;

            while (ip >= 0 && ip < code.Length)
            {
                if (++steps > maxSteps) throw new ExecutionLimitException(steps);

                ref readonly Instruction inst = ref code[ip];

                switch (inst.Code)
                {
                    // System
                    case OpCode.Nop: ip++; break;
                    case OpCode.Halt: return ExecutionState.Success;
                    case OpCode.Abort: throw new RuntimeException("Script explicitly aborted execution.");

                    // Memory
                    case OpCode.LoadConst: acc = consts[inst.Op1]; ip++; break;
                    case OpCode.LoadReg: acc = ctx.GetRegister(inst.Op1); ip++; break;
                    case OpCode.StoreReg: ctx.GetRegister(inst.Op1) = acc; ip++; break;
                    case OpCode.MoveReg: ctx.GetRegister(inst.Op2) = ctx.GetRegister(inst.Op1); ip++; break;

                    // Interop & Calling
                    case OpCode.PushArg: ctx.SetArg(inst.Op2, ctx.GetRegister(inst.Op1)); ip++; break;
                    case OpCode.CallExt:
                        var extMethod = (IMethodMetadata)meta[inst.Op1];
                        acc = extMethod.Invoke(ctx.TargetInstance, ctx.GetArgSpan(inst.Op2));
                        ip++; break;
                    case OpCode.CallInt:
                        ctx.PushFrame(ip + 1, inst.Op2); // Op2 = required registers in target func
                        ip = inst.Op1; // Op1 = target IP
                        break;
                    case OpCode.Ret:
                        ip = ctx.PopFrame();
                        break;
                    case OpCode.LoadProp:
                        var pGet = (IPropertyMetadata)meta[inst.Op1];
                        acc = pGet.Read(ctx.TargetInstance); ip++; break;
                    case OpCode.StoreProp:
                        var pSet = (IPropertyMetadata)meta[inst.Op1];
                        pSet.Write(ctx.TargetInstance, acc); ip++; break;

                    // Control Flow
                    case OpCode.Jump: ip = inst.Op1; break;
                    case OpCode.BranchTrue: ip = acc.AsBool ? inst.Op1 : inst.Op2; break;
                    case OpCode.BranchFalse: ip = !acc.AsBool ? inst.Op1 : inst.Op2; break;

                    // Relational
                    case OpCode.Eq: 
                        acc = Variant.FromBool(CompareEq(in acc, ref ctx.GetRegister(inst.Op1))); ip++; break;
                    case OpCode.Neq: 
                        acc = Variant.FromBool(!CompareEq(in acc, ref ctx.GetRegister(inst.Op1))); ip++; break;
                    case OpCode.Gt:
                        acc = Variant.FromBool(acc.Type == VariantType.Float 
                            ? acc.AsFloat > ctx.GetRegister(inst.Op1).AsFloat 
                            : acc.AsInt > ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.Lt:
                        acc = Variant.FromBool(acc.Type == VariantType.Float 
                            ? acc.AsFloat < ctx.GetRegister(inst.Op1).AsFloat 
                            : acc.AsInt < ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.Gte:
                        acc = Variant.FromBool(acc.Type == VariantType.Float 
                            ? acc.AsFloat >= ctx.GetRegister(inst.Op1).AsFloat 
                            : acc.AsInt >= ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.Lte:
                        acc = Variant.FromBool(acc.Type == VariantType.Float 
                            ? acc.AsFloat <= ctx.GetRegister(inst.Op1).AsFloat 
                            : acc.AsInt <= ctx.GetRegister(inst.Op1).AsInt); ip++; break;

                    // Logic
                    case OpCode.And: acc = Variant.FromBool(acc.AsBool && ctx.GetRegister(inst.Op1).AsBool); ip++; break;
                    case OpCode.Or: acc = Variant.FromBool(acc.AsBool || ctx.GetRegister(inst.Op1).AsBool); ip++; break;
                    case OpCode.Not: acc = Variant.FromBool(!acc.AsBool); ip++; break;

                    // Integer Math
                    case OpCode.AddInt: acc = Variant.FromInt(acc.AsInt + ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.SubInt: acc = Variant.FromInt(acc.AsInt - ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.MulInt: acc = Variant.FromInt(acc.AsInt * ctx.GetRegister(inst.Op1).AsInt); ip++; break;
                    case OpCode.DivInt:
                        int dI = ctx.GetRegister(inst.Op1).AsInt;
                        if (dI == 0) throw new DivideByZeroException();
                        acc = Variant.FromInt(acc.AsInt / dI); ip++; break;
                    case OpCode.ModInt:
                        int mI = ctx.GetRegister(inst.Op1).AsInt;
                        if (mI == 0) throw new DivideByZeroException();
                        acc = Variant.FromInt(acc.AsInt % mI); ip++; break;
                    case OpCode.NegInt: acc = Variant.FromInt(-acc.AsInt); ip++; break;

                    // Float Math
                    case OpCode.AddFloat: acc = Variant.FromFloat(acc.AsFloat + ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
                    case OpCode.SubFloat: acc = Variant.FromFloat(acc.AsFloat - ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
                    case OpCode.MulFloat: acc = Variant.FromFloat(acc.AsFloat * ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
                    case OpCode.DivFloat:
                        float dF = ctx.GetRegister(inst.Op1).AsFloat;
                        if (dF == 0f) throw new DivideByZeroException();
                        acc = Variant.FromFloat(acc.AsFloat / dF); ip++; break;
                    case OpCode.ModFloat:
                        float mF = ctx.GetRegister(inst.Op1).AsFloat;
                        if (mF == 0f) throw new DivideByZeroException();
                        acc = Variant.FromFloat(acc.AsFloat % mF); ip++; break;
                    case OpCode.NegFloat: acc = Variant.FromFloat(-acc.AsFloat); ip++; break;

                    // Vector Math
                    case OpCode.AddVec:
                        ref Variant addV = ref ctx.GetRegister(inst.Op1);
                        acc = Variant.FromVector(acc.X + addV.X, acc.Y + addV.Y, acc.Z + addV.Z, acc.W + addV.W); ip++; break;
                    case OpCode.SubVec:
                        ref Variant subV = ref ctx.GetRegister(inst.Op1);
                        acc = Variant.FromVector(acc.X - subV.X, acc.Y - subV.Y, acc.Z - subV.Z, acc.W - subV.W); ip++; break;
                    case OpCode.MulVec:
                        float scalar = ctx.GetRegister(inst.Op1).AsFloat;
                        acc = Variant.FromVector(acc.X * scalar, acc.Y * scalar, acc.Z * scalar, acc.W * scalar); ip++; break;

                    // Strings
                    case OpCode.Concat:
                        acc = Variant.FromObject(acc.GetAsString() + ctx.GetRegister(inst.Op1).GetAsString()); ip++; break;

                    // Collections
                    case OpCode.NewArr:
                        acc = Variant.FromObject(new Variant[acc.AsInt]); ip++; break;
                    case OpCode.LdLen:
                        var arrL = (Variant[])acc.AsObject!;
                        acc = Variant.FromInt(arrL.Length); ip++; break;
                    case OpCode.LdElem:
                        var arr = (Variant[])ctx.GetRegister(inst.Op1).AsObject!;
                        if (acc.AsInt < 0 || acc.AsInt >= arr.Length) throw new OutOfBoundsException(acc.AsInt, arr.Length);
                        acc = arr[acc.AsInt]; ip++; break;
                    case OpCode.StElem:
                        var arrDest = (Variant[])ctx.GetRegister(inst.Op1).AsObject!;
                        int idx = ctx.GetRegister(inst.Op2).AsInt;
                        if (idx < 0 || idx >= arrDest.Length) throw new OutOfBoundsException(idx, arrDest.Length);
                        arrDest[idx] = acc; ip++; break;

                    // Casts
                    case OpCode.ConvIntToFloat: acc = Variant.FromFloat((float)acc.AsInt); ip++; break;
                    case OpCode.ConvFloatToInt: acc = Variant.FromInt((int)acc.AsFloat); ip++; break;
                    case OpCode.ConvToStr: acc = Variant.FromObject(acc.GetAsString()); ip++; break;

                    default:
                        throw new RuntimeException($"Illegal instruction '{inst.Code}' encountered at IP {ip}.");
                }
            }

            return ExecutionState.Failure;
        }

        private static bool CompareEq(in Variant a, ref Variant b)
        {
            if (a.Type != b.Type) return false;
            return a.Type switch
            {
                VariantType.Int => a.AsInt == b.AsInt,
                VariantType.Float => a.AsFloat == b.AsFloat,
                VariantType.Bool => a.AsBool == b.AsBool,
                VariantType.Null => true,
                VariantType.Vector => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W,
                _ => object.Equals(a.AsObject, b.AsObject)
            };
        }
    }
}