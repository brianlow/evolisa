#pragma once

class NativeCode
{
public:
	NativeCode(void);
	~NativeCode(void);

	//2009-01-01 changed datatype from "unsigned long" to "unsigned long long"
	static unsigned long long fastImageCompare( void*, void*, int );
};
