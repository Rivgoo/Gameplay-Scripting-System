using GSS.Kernel.Api.Validation;
using GSS.Kernel.Primitives;

namespace GSS.Kernel.Api.Builders
{
    internal sealed class ParamBuilder1<TInstance, T1, TResult> : IParamBuilder1<TInstance, T1, TResult>
    {
        private readonly MethodMetadata<TInstance, TResult> _meta;
        private readonly Action<T1>? _v1;

        public ParamBuilder1(MethodMetadata<TInstance, TResult> meta, Action<T1>? v1) { _meta = meta; _v1 = v1; }

        public IParamBuilder2<TInstance, T1, T2, TResult> Param<T2>(string name, Action<ValidationBuilder<T2>>? validate = null)
        {
            _meta.AddArgCount();
            return new ParamBuilder2<TInstance, T1, T2, TResult>(_meta, _v1, MethodMetadata<TInstance, TResult>.CompileValidator(validate));
        }

        public void Handler(Action<TInstance, T1> action)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                action((TInstance)inst, a1);
                return Variant.Null;
            });
        }

        public void Handler(Func<TInstance, T1, TResult> func)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                return VariantExtensions.Pack(func((TInstance)inst, a1));
            });
        }
    }

    internal sealed class ParamBuilder2<TInstance, T1, T2, TResult> : IParamBuilder2<TInstance, T1, T2, TResult>
    {
        private readonly MethodMetadata<TInstance, TResult> _meta;
        private readonly Action<T1>? _v1; private readonly Action<T2>? _v2;

        public ParamBuilder2(MethodMetadata<TInstance, TResult> meta, Action<T1>? v1, Action<T2>? v2) { _meta = meta; _v1 = v1; _v2 = v2; }

        public IParamBuilder3<TInstance, T1, T2, T3, TResult> Param<T3>(string name, Action<ValidationBuilder<T3>>? validate = null)
        {
            _meta.AddArgCount();
            return new ParamBuilder3<TInstance, T1, T2, T3, TResult>(_meta, _v1, _v2, MethodMetadata<TInstance, TResult>.CompileValidator(validate));
        }

        public void Handler(Action<TInstance, T1, T2> action)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                action((TInstance)inst, a1, a2);
                return Variant.Null;
            });
        }

        public void Handler(Func<TInstance, T1, T2, TResult> func)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                return VariantExtensions.Pack(func((TInstance)inst, a1, a2));
            });
        }
    }

    internal sealed class ParamBuilder3<TInstance, T1, T2, T3, TResult> : IParamBuilder3<TInstance, T1, T2, T3, TResult>
    {
        private readonly MethodMetadata<TInstance, TResult> _meta;
        private readonly Action<T1>? _v1; private readonly Action<T2>? _v2; private readonly Action<T3>? _v3;

        public ParamBuilder3(MethodMetadata<TInstance, TResult> meta, Action<T1>? v1, Action<T2>? v2, Action<T3>? v3) { _meta = meta; _v1 = v1; _v2 = v2; _v3 = v3; }

        public IParamBuilder4<TInstance, T1, T2, T3, T4, TResult> Param<T4>(string name, Action<ValidationBuilder<T4>>? validate = null)
        {
            _meta.AddArgCount();
            return new ParamBuilder4<TInstance, T1, T2, T3, T4, TResult>(_meta, _v1, _v2, _v3, MethodMetadata<TInstance, TResult>.CompileValidator(validate));
        }

        public void Handler(Action<TInstance, T1, T2, T3> action)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                action((TInstance)inst, a1, a2, a3);
                return Variant.Null;
            });
        }

        public void Handler(Func<TInstance, T1, T2, T3, TResult> func)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                return VariantExtensions.Pack(func((TInstance)inst, a1, a2, a3));
            });
        }
    }

    internal sealed class ParamBuilder4<TInstance, T1, T2, T3, T4, TResult> : IParamBuilder4<TInstance, T1, T2, T3, T4, TResult>
    {
        private readonly MethodMetadata<TInstance, TResult> _meta;
        private readonly Action<T1>? _v1; private readonly Action<T2>? _v2; private readonly Action<T3>? _v3; private readonly Action<T4>? _v4;

        public ParamBuilder4(MethodMetadata<TInstance, TResult> meta, Action<T1>? v1, Action<T2>? v2, Action<T3>? v3, Action<T4>? v4) { _meta = meta; _v1 = v1; _v2 = v2; _v3 = v3; _v4 = v4; }

        public IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult> Param<T5>(string name, Action<ValidationBuilder<T5>>? validate = null)
        {
            _meta.AddArgCount();
            return new ParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult>(_meta, _v1, _v2, _v3, _v4, MethodMetadata<TInstance, TResult>.CompileValidator(validate));
        }

        public void Handler(Action<TInstance, T1, T2, T3, T4> action)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                var a4 = args[3].Unbox<T4>(); _v4?.Invoke(a4);
                action((TInstance)inst, a1, a2, a3, a4);
                return Variant.Null;
            });
        }

        public void Handler(Func<TInstance, T1, T2, T3, T4, TResult> func)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                var a4 = args[3].Unbox<T4>(); _v4?.Invoke(a4);
                return VariantExtensions.Pack(func((TInstance)inst, a1, a2, a3, a4));
            });
        }
    }

    internal sealed class ParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult> : IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult>
    {
        private readonly MethodMetadata<TInstance, TResult> _meta;
        private readonly Action<T1>? _v1; private readonly Action<T2>? _v2; private readonly Action<T3>? _v3; private readonly Action<T4>? _v4; private readonly Action<T5>? _v5;

        public ParamBuilder5(MethodMetadata<TInstance, TResult> meta, Action<T1>? v1, Action<T2>? v2, Action<T3>? v3, Action<T4>? v4, Action<T5>? v5) { _meta = meta; _v1 = v1; _v2 = v2; _v3 = v3; _v4 = v4; _v5 = v5; }

        public void Handler(Action<TInstance, T1, T2, T3, T4, T5> action)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                var a4 = args[3].Unbox<T4>(); _v4?.Invoke(a4);
                var a5 = args[4].Unbox<T5>(); _v5?.Invoke(a5);
                action((TInstance)inst, a1, a2, a3, a4, a5);
                return Variant.Null;
            });
        }

        public void Handler(Func<TInstance, T1, T2, T3, T4, T5, TResult> func)
        {
            _meta.SetInvoker((inst, args) => {
                var a1 = args[0].Unbox<T1>(); _v1?.Invoke(a1);
                var a2 = args[1].Unbox<T2>(); _v2?.Invoke(a2);
                var a3 = args[2].Unbox<T3>(); _v3?.Invoke(a3);
                var a4 = args[3].Unbox<T4>(); _v4?.Invoke(a4);
                var a5 = args[4].Unbox<T5>(); _v5?.Invoke(a5);
                return VariantExtensions.Pack(func((TInstance)inst, a1, a2, a3, a4, a5));
            });
        }
    }
}