// FIC.h

#pragma once

using namespace System;

namespace FIC {

	public ref class FastImageCompare
	{
	public:
		static double compare( void* p1, void* p2, int count )
		{
			return NativeCode::fastImageCompare( p1, p2, count );
		}
		static double compare( IntPtr p1, IntPtr p2, int count )
		{
			return NativeCode::fastImageCompare( p1.ToPointer(), p2.ToPointer(), count );
		}
	};
}
