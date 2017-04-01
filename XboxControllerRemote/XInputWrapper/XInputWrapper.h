// XInputWrapper.h

#pragma once

using namespace System;

namespace XInputWrapper {

	extern const short _LEFT_THUMB_DEADZONE;
	extern const short _RIGHT_THUMB_DEADZONE;
	extern const short _TRIGGER_THRESHOLD;

	public value class XInputGamepad
	{
	public:
		WORD wButtons;
		BYTE bLeftTrigger;
		BYTE bRightTrigger;
		SHORT sThumbLX;
		SHORT sThumbLY;
		SHORT sThumbRX;
		SHORT sThumbRY;
	};

	public value class XInputState
	{
	public:
		DWORD dwPacketNumber;
		XInputGamepad Gamepad;

		static XInputState XInputGetStateWrapper(DWORD dwUserIndex);

		static const short LEFT_THUMB_DEADZONE = _LEFT_THUMB_DEADZONE;
		static const short RIGHT_THUMB_DEADZONE = _RIGHT_THUMB_DEADZONE;
		static const short TRIGGER_THRESHOLD = _TRIGGER_THRESHOLD;
	};
}
