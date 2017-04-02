// This is the main DLL file.

#include "stdafx.h"

#include "XInputWrapper.h"

#pragma comment(lib, "User32.lib")
#pragma comment(lib, "Xinput.lib")

namespace XInputWrapper {

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

	void MouseEventWrapper::MouseEvent(DWORD dwFlags, DWORD dx, DWORD dy, DWORD dwData, ULONG_PTR dwExtraInfo) {
		mouse_event(dwFlags, dx, dy, dwData, dwExtraInfo);
	}

}
