#pragma once
#include <iostream>	
#include <string>
namespace vectors
{
	struct Vector3
	{
		float X;
		float Y;
		float Z;

		Vector3() : X(0), Y(0), Z(0) {}
		Vector3(float x, float y, float z) : X(x), Y(y), Z(z) {}

		// Operator overloads
		Vector3 operator+(const Vector3& other) const {
			return Vector3(X + other.X, Y + other.Y, Z + other.Z);
		}

		Vector3 operator-(const Vector3& other) const {
			return Vector3(X - other.X, Y - other.Y, Z - other.Z);
		}

		Vector3 operator*(float scalar) const {
			return Vector3(X * scalar, Y * scalar, Z * scalar);
		}

		Vector3 operator/(float scalar) const {
			return Vector3(X / scalar, Y / scalar, Z / scalar);
		}

		Vector3& operator+=(const Vector3& other) {
			X += other.X; Y += other.Y; Z += other.Z;
			return *this;
		}

		Vector3& operator-=(const Vector3& other) {
			X -= other.X; Y -= other.Y; Z -= other.Z;
			return *this;
		}

		Vector3& operator*=(float scalar) {
			X *= scalar; Y *= scalar; Z *= scalar;
			return *this;
		}

		Vector3& operator/=(float scalar) {
			X /= scalar; Y /= scalar; Z /= scalar;
			return *this;
		}
		// Vector magnitude (length)
		float magnitude() const {
			return std::sqrt(X * X + Y * Y + Z * Z);
		}

		// Normalize the vector
		Vector3 normalized() const {
			float mag = magnitude();
			if (mag == 0) return Vector3(0, 0, 0);
			return *this / mag;
		}

		// Dot product
		float dot(const Vector3& other) const {
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		// Cross product
		Vector3 cross(const Vector3& other) const {
			return Vector3(
				Y * other.Z - Z * other.Y,
				Z * other.X - X * other.Z,
				X * other.Y - Y * other.X
			);
		}

		// Debug print
		void print() const {
			std::cout << "Vector3(" << X << ", " << Y << ", " << Z << ")" << std::endl;
		}
	};
}
