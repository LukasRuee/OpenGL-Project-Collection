#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <iostream>	
#include <string>
// TODO: Add includes

void framebufferSizeCallback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow* window);
void hexToOpenGL_RGB(const std::string& hex, float& r, float& g, float& b);
double sinPos(double x);

// settings

	// TODO: Add screen constants
static int WindowWidth = 1600;
static int WindowLength = 900;


	int main()
	{
		//-------------------------------
		// Initialize and configure
		// ------------------------------
		// TODO: Set major and minor OpenGL version and profile
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	
		// ------------------------------
		// GLFW Window creation
		// ------------------------------
		// TODO: Create and display window

		GLFWwindow* window = glfwCreateWindow(WindowWidth, WindowLength, "Super cool and epic OpenGL window", NULL, NULL);
		if (window == NULL)
		{
			std::cout << "Failed to create GLFW window" << std::endl;
			glfwTerminate();
			return -1;
		}
		glfwMakeContextCurrent(window);

		// ------------------------------
		// GLAD: load all OpenGL function pointers
		// ------------------------------
		if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
		{
			std::cout << "Failed to initialize GLAD" << std::endl;
			return -1;
		}
		// ------------------------------
		// Render Loop
		// ------------------------------

		double currentFrameTime = glfwGetTime();
		double lastFrameTime = currentFrameTime;
		double elapsedTime = 0;
		double deltaTime = 0;
		float r, g, b;

		while (!glfwWindowShouldClose(window))
		{
			double currentFrameTime = glfwGetTime();
			deltaTime = currentFrameTime - lastFrameTime;
			lastFrameTime = currentFrameTime;

			elapsedTime += deltaTime;

			//------------------------------
			// Input
			//------------------------------
			processInput(window);

			//------------------------------
			// Render
			//------------------------------
			// TODO: Set clear color and clear type
			
			//Adding 1.0f to shift range from [-1, 1] to [0, 2].
			//Div by 2.0f scaling down to [0, 2] to [0, 1].
			r = (float)sinPos(elapsedTime + 0);
			g = (float)sinPos(elapsedTime + 2);
			b = (float)sinPos(elapsedTime + 4);

			hexToOpenGL_RGB("FF0000", r, g, b);
			r *= (float)sinPos(elapsedTime);

			glClearColor(r, g, b, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT);

			//------------------------------
			// glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
			//------------------------------
			// TODO: Swap frame buffers (back to front)
			glfwSwapBuffers(window);

			glfwPollEvents();
		}

	//------------------------------
	// glfw: terminate, clearing all previously allocated GLFW resources.
	//------------------------------
	glfwTerminate();
	return 0;
}

//------------------------------
// Process all input: query GLFW whether relevant keys are pressed/released this frame and react accordingly
//------------------------------
// TODO: Implement processInput(..)
void processInput(GLFWwindow* window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
	{
		glfwSetWindowShouldClose(window, true);
	}
}

//------------------------------
// glfw: whenever the window size changed (by OS or user resize input) this callback function executes
//------------------------------
// TODO: Implement framebufferSizeCallback(..)
void framebufferSizeCallback(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
}

void hexToOpenGL_RGB(const std::string& hex, float& r, float& g, float& b)
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

double sinPos(double x)
{
	return (sin(x) + 1.0f) / 2.0f;
}