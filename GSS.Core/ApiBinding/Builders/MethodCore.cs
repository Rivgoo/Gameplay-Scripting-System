using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Validation;
using System.Linq.Expressions;
using System.Reflection;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class MethodCore<TInstance, TResult>
	{
		public string Name { get; set; }
		public readonly List<(Type Type, Delegate? ValidationDelegate)> Params = new();
		public GssMemberInvoker? Invoker;

		public MethodCore(string name) => Name = name;

		public void AddParam<T>(string name, Action<ValidationBuilder<T>>? validate)
		{
			Delegate? validationDelegate = null;

			if (validate != null)
			{
				var vb = new ValidationBuilder<T>();
				validate(vb);
				var validators = vb.Validators.ToArray();

				if (validators.Length > 0)
				{
					Action<T> action = (val) =>
					{
						for (int i = 0; i < validators.Length; i++)
						{
							validators[i].Validate(val);
						}
					};
					validationDelegate = action;
				}
			}

			Params.Add((typeof(T), validationDelegate));
		}

		public void Compile(Delegate del)
		{
			var instanceParam = Expression.Parameter(typeof(object), "instance");
			var argsParam = Expression.Parameter(typeof(ReadOnlySpan<GssValue>), "args");
			var castInstance = Expression.Convert(instanceParam, typeof(TInstance));

			var blockVariables = new List<ParameterExpression>();
			var blockExpressions = new List<Expression>();
			var callArgs = new List<Expression> { castInstance };

			for (int i = 0; i < Params.Count; i++)
			{
				var paramType = Params[i].Type;
				var getArgMethod = typeof(MethodCore<TInstance, TResult>).GetMethod(nameof(GetArg), BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(paramType);
				var rawArg = Expression.Call(getArgMethod, argsParam, Expression.Constant(i));

				var argVar = Expression.Parameter(paramType, $"arg{i}");
				blockVariables.Add(argVar);
				blockExpressions.Add(Expression.Assign(argVar, rawArg));

				var validationDelegate = Params[i].ValidationDelegate;
				if (validationDelegate != null)
				{
					var invokeMethod = validationDelegate.GetType().GetMethod("Invoke")!;
					var invokeExpr = Expression.Call(Expression.Constant(validationDelegate), invokeMethod, argVar);
					blockExpressions.Add(invokeExpr);
				}

				callArgs.Add(argVar);
			}

			var call = Expression.Invoke(Expression.Constant(del), callArgs);

			if (typeof(TResult) == typeof(VoidResult))
			{
				blockExpressions.Add(call);
				blockExpressions.Add(Expression.Constant(GssValue.Null));
			}
			else
			{
				var packMethod = typeof(MethodCore<TInstance, TResult>).GetMethod(nameof(PackGeneric), BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(typeof(TResult));
				var packedResult = Expression.Call(packMethod, call);
				blockExpressions.Add(packedResult);
			}

			var body = Expression.Block(blockVariables, blockExpressions);
			Invoker = Expression.Lambda<GssMemberInvoker>(body, instanceParam, argsParam).Compile();
		}

		public static T GetArg<T>(ReadOnlySpan<GssValue> args, int index) => args[index].Unbox<T>();

		public static GssValue PackGeneric<TRet>(TRet obj)
		{
			if (typeof(TRet) == typeof(int)) return GssValue.FromInt((int)(object)obj!);
			if (typeof(TRet) == typeof(float)) return GssValue.FromFloat((float)(object)obj!);
			if (typeof(TRet) == typeof(bool)) return GssValue.FromBool((bool)(object)obj!);
			if (typeof(TRet) == typeof(double)) return GssValue.FromDouble((double)(object)obj!);
			if (typeof(TRet) == typeof(long)) return GssValue.FromLong((long)(object)obj!);

			return GssValue.FromObject(obj);
		}
	}
}