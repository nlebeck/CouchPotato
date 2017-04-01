// This is the main DLL file.

#include "stdafx.h"

#include "XInputWrapper.h"

#pragma comment(lib, "Xinput.lib")

namespace XInputWrapper {

	const short _LEFT_THUMB_DEADZONE = XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE;
	const short _RIGHT_THUMB_DEADZONE = XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE;
	const short _TRIGGER_THRESHOLD = XINPUT_GAMEPAD_TRIGGER_THRESHOLD;

	XInputState XInputState::XInputGetStateWrapper(DWORD dwUserIndex) {
		XINPUT_STATE state;
		XInputGetState(dwUserIndex, &state);
		XInputState result;
		result.dwPacketNumber = state.dwPacketNumber;
		result.Gamepad.bLeftTrigger = state.Gamepad.bLeftTrigger;
		result.Gamepad.bRightTrigger = state.Gamepad.bRightTrigger;
		result.Gamepad.sThumbLX = state.Gamepad.sThumbLX;
		result.Gamepad.sThumbLY = state.Gamepad.sThumbLY;
		result.Gamepad.sThumbRX = state.Gamepad.sThumbRX;
		result.Gamepad.sThumbRY = state.Gamepad.sThumbRY;
		result.Gamepad.wButtons = state.Gamepad.wButtons;
		return result;
	}
}
