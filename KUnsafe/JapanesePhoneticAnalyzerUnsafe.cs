using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KUnsafe;


public static class JapanesePhoneticAnalyzerUnsafe
{
	public static IObjectReference JapanesePhoneticAnalyzerObj { get; private set; } = null;


	public static void Initialize()
	{
		if (JapanesePhoneticAnalyzerObj != null)
		{
			return;
		}

		JapanesePhoneticAnalyzer.GetWords("");
		var field = typeof(JapanesePhoneticAnalyzer).GetField("___objRef_global__Windows_Globalization_IJapanesePhoneticAnalyzerStatics",
				BindingFlags.Static |
				BindingFlags.NonPublic);

		JapanesePhoneticAnalyzerObj = (IObjectReference)field.GetValue(null);


	}

	public unsafe static IReadOnlyList<global::Windows.Globalization.JapanesePhoneme> GetWords(string input)
	{
        if (JapanesePhoneticAnalyzerObj == null)
        {
			throw new InvalidOperationException("No");
        }

        var _obj = JapanesePhoneticAnalyzerObj;
		IntPtr thisPtr = _obj.ThisPtr;
		IntPtr intPtr = default(IntPtr);
		try
		{
			MarshalString.Pinnable p = new MarshalString.Pinnable(input);
			fixed (char* ptr = p)
			{
				void* ptr2 = ptr;
				ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<IntPtr, IntPtr, out IntPtr, int>)(*(IntPtr*)((nint)(*(IntPtr*)(void*)thisPtr) + (nint)6 * (nint)sizeof(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, out IntPtr, int>))))(thisPtr, MarshalString.GetAbi(ref p), out intPtr));
				return MarshalInterface<IReadOnlyList<global::Windows.Globalization.JapanesePhoneme>>.FromAbi(intPtr);
			}
		}
		finally
		{
			MarshalInterface<IReadOnlyList<global::Windows.Globalization.JapanesePhoneme>>.DisposeAbi(intPtr);
		}
	}

	public unsafe static IReadOnlyList<global::Windows.Globalization.JapanesePhoneme> GetWords(string input, bool monoRuby)
	{
		if (JapanesePhoneticAnalyzerObj == null)
		{
			throw new InvalidOperationException("No");
		}

		var _obj = JapanesePhoneticAnalyzerObj;
		IntPtr thisPtr = _obj.ThisPtr;
		IntPtr intPtr = default(IntPtr);
		try
		{
			MarshalString.Pinnable p = new MarshalString.Pinnable(input);
			fixed (char* ptr = p)
			{
				void* ptr2 = ptr;
				ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte, out IntPtr, int>)(*(IntPtr*)((nint)(*(IntPtr*)(void*)thisPtr) + (nint)7 * (nint)sizeof(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte, out IntPtr, int>))))(thisPtr, MarshalString.GetAbi(ref p), monoRuby ? ((byte)1) : ((byte)0), out intPtr));
				return MarshalInterface<IReadOnlyList<global::Windows.Globalization.JapanesePhoneme>>.FromAbi(intPtr);
			}
		}
		finally
		{
			MarshalInterface<IReadOnlyList<global::Windows.Globalization.JapanesePhoneme>>.DisposeAbi(intPtr);
		}
	}
}
