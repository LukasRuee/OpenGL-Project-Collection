#include "Utility.h"

double Utility::sinPos(double x)
{
	return (sin(x) + 1.0f) / 2.0f;
}
void Utility::hexTo_RGB(const std::string& hex, float& r, float& g, float& b)
{
	//Stupidity test
	if (hex.length() != 6) return;

	//Char to int magic + error check (Just set to 0 instead of crashing)
	auto hexToDec = [](char c) -> int
		{
			if (c >= '0' && c <= '9') return c - '0';
			if (c >= 'A' && c <= 'F') return c - 'A' + 10;
			if (c >= 'a' && c <= 'f') return c - 'a' + 10;
			return 0;
		};

	//Combining sets of hex digits
	//shift first one by 4 (left nibble), add second one (right nibble), then divide by 255 (To make it OpenGL conform)

	// Split hex from "FF" into "F" and "F" = 0b1111 and 0b1111
	// (0b1111 << 4) = 0b11110000 (high nibble) | 0b1111 = 0b11111111
	// OR can be used to "glue" them together
	r = ((hexToDec(hex[0]) << 0b0100) | hexToDec(hex[1])) / static_cast<float>(255);
	g = ((hexToDec(hex[2]) << 0b0100) | hexToDec(hex[3])) / static_cast<float>(255);
	b = ((hexToDec(hex[4]) << 0b0100) | hexToDec(hex[5])) / static_cast<float>(255);
}
void Utility::hexTo_RGB(const int hex, float& r, float& g, float& b)
{
	// Extract RGB components using bitwise operations
	r = ((hex >> 0b1111) & 0xFF) / 255.0f;
	g = ((hex >> 0b1000) & 0xFF) / 255.0f;
	b = (hex & 0xFF) / 255.0f;
}