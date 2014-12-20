#include "StdAfx.h"
#include "NativeCode.h"

/*
Written by Dan Byström
http://danbystrom.se/2008/12/22/optimizing-away-ii/
*/

NativeCode::NativeCode(void)
{
}

NativeCode::~NativeCode(void)
{
}

unsigned long long NativeCode::fastImageCompare( void* p1, void* p2, int count )
{
	int high32 = 0;

	_asm
	{
		push	ebx
		push	esi
		push	edi

		mov		esi, p1
		mov		edi, p2
		xor		eax, eax
again:
		dec		count
		js		done

		movzx	ebx, byte ptr [esi]
		movzx	edx, byte ptr [edi]
		sub		edx, ebx
		imul	edx, edx

		movzx	ebx, byte ptr [esi+1]
		movzx	ecx, byte ptr [edi+1]
		sub		ebx, ecx
		imul	ebx, ebx
		add		edx, ebx

		movzx	ebx, byte ptr [esi+2]
		movzx	ecx, byte ptr [edi+2]
		sub		ebx, ecx
		imul	ebx, ebx
		add		edx, ebx

		add		esi, 4
		add		edi, 4

		add		eax, edx
		jnc		again

		inc		high32
		jmp		again
done:
		mov		edx, high32

		pop		edi
		pop		esi
		pop		ebx
	}

}

