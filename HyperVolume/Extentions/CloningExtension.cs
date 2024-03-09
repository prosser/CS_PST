﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PST.HyperVolume.Extentions {
	static public class CloningExtension {
		static public IHyperVolume<T> Clone<T>(this IHyperVolume<T> instance, ThreadingOptions? threadingOptions = null) {
			if (instance is null)
				throw new ArgumentNullException("instance", "Instance cannot be null");

			Type instanceType = instance.GetType();
			Type tType = typeof(T);
			bool tIsValueType = tType.IsValueType;

			int instanceArea = instance.Area;
			int[] instanceShape = instance.Shape.ToArray();

			IHyperVolume<T>? newVolume = (IHyperVolume<T>?)Activator.CreateInstance(instanceType, instanceShape) ??
				throw new InvalidOperationException("Failed to create new instance of IHyperVolume");

			newVolume.TypeInterpolationMethod = instance.TypeInterpolationMethod;

			newVolume.Foreach((volume, index) => {
				T thisValue = instance[index];

				if (tIsValueType)
					volume[index] = thisValue;
				else
					throw new NotImplementedException("Cloning of non-value types is not yet implemented");

			}, null, threadingOptions);

			return newVolume;
		}

		static public async Task<IHyperVolume<T>> CloneAsync<T>(this IHyperVolume<T> instance, ThreadingOptions? threadingOptions = null) =>
			await Task.Run(() => Clone(instance, threadingOptions));
	}
}
