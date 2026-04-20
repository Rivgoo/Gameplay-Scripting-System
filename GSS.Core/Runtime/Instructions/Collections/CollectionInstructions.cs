using System.Collections;
using System.Collections.Generic;
using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Helpers;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Collections
{
	public sealed class CreateArrayInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int size = ctx.Accumulator.AsInt;
			if (size < 0) throw new GssRuntimeException($"Cannot create an array with a negative size: {size}.");

			var array = new GssValue[size];
			ctx.Accumulator = GssValue.FromObject(array);
			
			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class CreateListInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var list = new List<GssValue>();
			ctx.Accumulator = GssValue.FromObject(list);
			
			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class GetCollectionLengthInstruction : IInstruction
	{
		private readonly int _collectionReg;

		public GetCollectionLengthInstruction(int collectionReg)
		{
			_collectionReg = collectionReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var collectionValue = ctx.GetRegister(_collectionReg);
			object? obj = collectionValue.AsObject;

			if (obj is ICollection collection)
			{
				ctx.Accumulator = GssValue.FromInt(collection.Count);
			}
			else if (obj is GssValue[] gssArray)
			{
				ctx.Accumulator = GssValue.FromInt(gssArray.Length);
			}
			else
			{
				throw new GssInvalidCollectionException(obj?.GetType().Name ?? "null");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class LoadElementInstruction : IInstruction
	{
		private readonly int _collectionReg;

		public LoadElementInstruction(int collectionReg)
		{
			_collectionReg = collectionReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int index = ctx.Accumulator.AsInt;
			object? col = ctx.GetRegister(_collectionReg).AsObject;

			if (col is GssValue[] gssArray)
			{
				EnsureBounds(index, gssArray.Length);
				ctx.Accumulator = gssArray[index];
			}
			else if (col is List<GssValue> gssList)
			{
				EnsureBounds(index, gssList.Count);
				ctx.Accumulator = gssList[index];
			}
			else if (col is int[] intArray)
			{
				EnsureBounds(index, intArray.Length);
				ctx.Accumulator = GssValue.FromInt(intArray[index]);
			}
			else if (col is List<int> intList)
			{
				EnsureBounds(index, intList.Count);
				ctx.Accumulator = GssValue.FromInt(intList[index]);
			}
			else if (col is float[] floatArray)
			{
				EnsureBounds(index, floatArray.Length);
				ctx.Accumulator = GssValue.FromFloat(floatArray[index]);
			}
			else if (col is List<float> floatList)
			{
				EnsureBounds(index, floatList.Count);
				ctx.Accumulator = GssValue.FromFloat(floatList[index]);
			}
			else if (col is bool[] boolArray)
			{
				EnsureBounds(index, boolArray.Length);
				ctx.Accumulator = GssValue.FromBool(boolArray[index]);
			}
			else if (col is IList list)
			{
				EnsureBounds(index, list.Count);
				ctx.Accumulator = GssValuePacker.Pack(list[index]);
			}
			else
			{
				throw new GssInvalidCollectionException(col?.GetType().Name ?? "null");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}

		private static void EnsureBounds(int index, int length)
		{
			if (index < 0 || index >= length) throw new GssIndexOutOfRangeException(index, length);
		}
	}

	public sealed class StoreElementInstruction : IInstruction
	{
		private readonly int _collectionReg;
		private readonly int _indexReg;

		public StoreElementInstruction(int collectionReg, int indexReg)
		{
			_collectionReg = collectionReg;
			_indexReg = indexReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int index = ctx.GetRegister(_indexReg).AsInt;
			object? col = ctx.GetRegister(_collectionReg).AsObject;
			var value = ctx.Accumulator;

			if (col is GssValue[] gssArray)
			{
				EnsureBounds(index, gssArray.Length);
				gssArray[index] = value;
			}
			else if (col is List<GssValue> gssList)
			{
				EnsureBounds(index, gssList.Count);
				gssList[index] = value;
			}
			else if (col is int[] intArray)
			{
				EnsureBounds(index, intArray.Length);
				intArray[index] = value.AsInt;
			}
			else if (col is List<int> intList)
			{
				EnsureBounds(index, intList.Count);
				intList[index] = value.AsInt;
			}
			else if (col is float[] floatArray)
			{
				EnsureBounds(index, floatArray.Length);
				floatArray[index] = value.AsFloat;
			}
			else if (col is List<float> floatList)
			{
				EnsureBounds(index, floatList.Count);
				floatList[index] = value.AsFloat;
			}
			else if (col is bool[] boolArray)
			{
				EnsureBounds(index, boolArray.Length);
				boolArray[index] = value.AsBool;
			}
			else if (col is IList list) // Fallback
			{
				EnsureBounds(index, list.Count);
				if (list.IsFixedSize && list is not Array) throw new GssRuntimeException("Collection is fixed-size and cannot be modified.");
				list[index] = value.ToBoxedValue();
			}
			else
			{
				throw new GssInvalidCollectionException(col?.GetType().Name ?? "null");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}

		private static void EnsureBounds(int index, int length)
		{
			if (index < 0 || index >= length) throw new GssIndexOutOfRangeException(index, length);
		}
	}

	public sealed class AddToListInstruction : IInstruction
	{
		private readonly int _collectionReg;

		public AddToListInstruction(int collectionReg)
		{
			_collectionReg = collectionReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			object? col = ctx.GetRegister(_collectionReg).AsObject;
			var value = ctx.Accumulator;

			if (col is List<GssValue> gssList)
			{
				gssList.Add(value);
			}
			else if (col is List<int> intList)
			{
				intList.Add(value.AsInt);
			}
			else if (col is List<float> floatList)
			{
				floatList.Add(value.AsFloat);
			}
			else if (col is IList list && !list.IsFixedSize)
			{
				list.Add(value.ToBoxedValue());
			}
			else
			{
				throw new GssRuntimeException($"Object of type '{col?.GetType().Name}' is not a dynamically resizable list.");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class RemoveFromListAtInstruction : IInstruction
	{
		private readonly int _collectionReg;

		public RemoveFromListAtInstruction(int collectionReg)
		{
			_collectionReg = collectionReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int index = ctx.Accumulator.AsInt;
			object? col = ctx.GetRegister(_collectionReg).AsObject;

			if (col is IList list && !list.IsFixedSize)
			{
				if (index < 0 || index >= list.Count) throw new GssIndexOutOfRangeException(index, list.Count);
				list.RemoveAt(index);
			}
			else
			{
				throw new GssRuntimeException($"Object of type '{col?.GetType().Name}' is not a dynamically resizable list.");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class ClearCollectionInstruction : IInstruction
	{
		private readonly int _collectionReg;

		public ClearCollectionInstruction(int collectionReg)
		{
			_collectionReg = collectionReg;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			object? col = ctx.GetRegister(_collectionReg).AsObject;

			if (col is IList list && !list.IsFixedSize)
			{
				list.Clear();
			}
			else
			{
				throw new GssRuntimeException($"Object of type '{col?.GetType().Name}' is not a dynamically resizable list.");
			}

			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}