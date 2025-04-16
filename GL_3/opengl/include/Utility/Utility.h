#pragma once
#include <string>

static class Utility
{

public:

	/// <summary>
	/// Returns Sinus in Positive range
	/// </summary>
	static double sinPos(double x);

	/// <summary>
	/// Returns from hex number the rgb value
	/// </summary>
	/// <param name="hex">Hex style = "FFFFFF"</param>
	static void hexTo_RGB(const std::string& hex, float& r, float& g, float& b);

	/// <summary>
	/// Returns from hex number the rgb value
	/// </summary>
	/// <param name="hex">Hex style = 0xFFFFFF</param>
	static void hexTo_RGB(const int hex, float& r, float& g, float& b);
};

