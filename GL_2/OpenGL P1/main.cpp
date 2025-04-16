#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <iostream>	
#include <string>

#include "Vector.h"
using vectors::Vector3;

// TODO: Add includes

void framebufferSizeCallback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow* window);
void hexToOpenGL_RGB(const std::string& hex, float& r, float& g, float& b);
double sinPos(double x);

void bind(unsigned int VBO, unsigned int EBO, float vertices[], unsigned int verticesSize, unsigned int indices[], unsigned int indicesSize);
void bind(unsigned int VBO, unsigned int EBO, vectors::Vector3 vertices[], size_t vertexCount, unsigned int indices[], unsigned int indicesSize);
// settings

	// TODO: Add screen constants
static int WindowWidth = 1600;
static int WindowLength = 900;

//Shader
const char* vertexShaderSource = "#version 400 core\n"
"layout (location = 0) in vec3 aPos;\n"
"void main()\n"
"{\n"
" gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n"
"}\0";
const char* fragmentShaderSource = "#version 400 core\n"
"out vec4 FragColor;\n"
"void main()\n"
"{\n"
" FragColor = vec4(0.0f, 0.0f, 1.0f, 1.0f);\n"
"}\0";

//Triangle
vectors::Vector3 vertices_triangle[] = {
	{ 0.0f,  0.0f, 0.0f },  // bottom left
	{ 1.0f,  0.0f, 0.0f },  // bottom right
	{ 0.0f,  1.0f, 0.0f }   // top center
};
unsigned int indices_triangle[] = {
	0, 1, 2
};

//Rectangle
float vertices_rectangle[] = {
	-0.5f, -0.5f, 0.0f,  // bottom left
	 0.5f, -0.5f, 0.0f,  // bottom right
	 0.5f,  0.5f, 0.0f,   // top right
	 -0.5f,  0.5f, 0.0f   // top left
};
unsigned int indices_rectangle[] = {
	0, 1, 2,
	2, 3, 0
};

//Cube
float vertices_cube[] = {
	// Bottom face
	-0.5f, -0.5f, 0.5f,  // bottom left near
	 0.5f, -0.5f, 0.5f,   // bottom right near
	 0.5f,  0.5f, 0.5f,   // top right near
	-0.5f,  0.5f, 0.5f,  // top left near

    // Top face
	- 0.5f, -0.5f,  -0.5f,  // bottom left far
	 0.5f, -0.5f,  -0.5f,   // bottom right far
	 0.5f,  0.5f,  -0.5f,   // top right far
	-0.5f,  0.5f,  -0.5f   // top left far
};
unsigned int indices_cube[] = {
	// Near face (bottom, front)
	0, 1, 2,
	2, 3, 0,

	// Far face (top, back)
	4, 5, 1,
	1, 0, 4,

	// Left face
	7, 6, 5,
	5, 4, 7,

	// Right face
	3, 2, 6,
	6, 7, 3,

	// Bottom face
	1, 5, 6,
	6, 2, 1,

	// Top face
	4, 0, 3,
	3, 7, 4
};

//Pyramid
float vertices_pyramid[] = {
	-0.5f, -0.5f, 0.5f,  // bottom left near
	 0.5f, -0.5f, 0.5f,   // bottom right near
	-0.5f, -0.5f,  -0.5f,  // bottom left far
	 0.5f, -0.5f,  -0.5f,   // bottom right far

	 0.0f,  0.5f, 0.0f,  // top center
};
unsigned int indices_pyramid[] = {
	0, 1, 2,	//Bottom A
	2, 3, 0,	//Bottom B
	0, 1, 4,	//Front
	1, 2, 4,	//Right
	2, 3, 4,	//Back
	3, 0, 4,	//Left
};

	int main()
	{
		// Initialize and configure
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	
		// GLFW Window creation
		GLFWwindow* window = glfwCreateWindow(WindowWidth, WindowLength, "Super cool and epic OpenGL window", NULL, NULL);
		if (window == NULL)
		{
			std::cout << "Failed to create GLFW window" << std::endl;
			glfwTerminate();
			return -1;
		}
		glfwMakeContextCurrent(window);

		// GLAD: load all OpenGL function pointers
		if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
		{
			std::cout << "Failed to initialize GLAD" << std::endl;
			return -1;
		}

#pragma region Vertex Shader
		// Compile vertex shader
		int vertexShader = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertexShader, 1, &vertexShaderSource, NULL);
		glCompileShader(vertexShader);
		// check for shader compile errors
		int success;
		char infoLog[512];
		glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(vertexShader, 512, NULL, infoLog);
			std::cout << "Exception: Vertex shader compilation failed.\n" << infoLog << std::endl;
		}
#pragma endregion
#pragma region Fragment Shader
		// Compile fragment shader
		int fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragmentShader, 1, &fragmentShaderSource, NULL);
		glCompileShader(fragmentShader);
		// check for shader compile errors
		glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(fragmentShader, 512, NULL, infoLog);
			std::cout << "Exception: Fragment shader compilation failed.\n" << infoLog << std::endl;
		}
#pragma endregion
#pragma region Build and Link
		// Link shaders
		int shaderProgram = glCreateProgram();
		glAttachShader(shaderProgram, vertexShader);
		glAttachShader(shaderProgram, fragmentShader);
		glLinkProgram(shaderProgram);
		// Check for linking errors
		glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
		if (!success)
		{
			glGetProgramInfoLog(shaderProgram, 512, NULL, infoLog);
			std::cout << " Exception: Program linking failed.\\n" << infoLog << std::endl;
		}
		// Free allocated memory for shaders
		glDeleteShader(vertexShader);
		glDeleteShader(fragmentShader);
#pragma endregion

		unsigned int VAO, VBO, EBO;

		glGenVertexArrays(1, &VAO);
		glBindVertexArray(VAO);// Bind after generation, rebind when needed again

		glGenBuffers(1, &VBO);
		glGenBuffers(1, &EBO);

		//Shapes
		//bind(VBO, EBO, vertices_triangle, sizeof(vertices_triangle), indices_triangle, sizeof(indices_triangle));
		//bind(VBO, EBO, vertices_rectangle, sizeof(vertices_rectangle), indices_rectangle, sizeof(indices_rectangle));
		bind(VBO, EBO, vertices_cube, sizeof(vertices_cube), indices_cube, sizeof(indices_cube));
		//bind(VBO, EBO, vertices_pyramid, sizeof(vertices_pyramid), indices_pyramid, sizeof(indices_pyramid));

		glUseProgram(shaderProgram);// Which shader program should be used
		int size; glGetBufferParameteriv(GL_ELEMENT_ARRAY_BUFFER, GL_BUFFER_SIZE, &size);

		glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
		//glPolygonMode(GL_FRONT_AND_BACK, 0x1B02); //Works too lol
		//glPolygonMode(GL_FRONT_AND_BACK, GL_POINT);
		//glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

		double currentFrameTime = glfwGetTime();
		double lastFrameTime = currentFrameTime;
		double elapsedTime = 0;
		double deltaTime = 0;

		while (!glfwWindowShouldClose(window))
		{
			double currentFrameTime = glfwGetTime();
			deltaTime = currentFrameTime - lastFrameTime;
			lastFrameTime = currentFrameTime;

			elapsedTime += deltaTime;

			processInput(window);

			glClearColor(0.25f, 0.25f, 1, 0.25f);
			glClear(GL_COLOR_BUFFER_BIT);

			glDrawElements(GL_TRIANGLES, size / sizeof(GLuint), GL_UNSIGNED_INT, 0); // Draw Call

			glfwSwapBuffers(window);

			glfwPollEvents();
		}
	glfwTerminate();
	return 0;
}

void processInput(GLFWwindow* window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
	{
		glfwSetWindowShouldClose(window, true);
	}
}

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

void bind(unsigned int VBO, unsigned int EBO, float vertices[], unsigned int verticesSize, unsigned int indices[], unsigned int indicesSize)
{
	glBindBuffer(*vertices, *indices); //No need???

	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, verticesSize, vertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, indicesSize, indices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
}

void bind(unsigned int VBO, unsigned int EBO, vectors::Vector3 vertices[], size_t vertexCount, unsigned int indices[], unsigned int indicesSize)
{
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, vertexCount * sizeof(vectors::Vector3), vertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, indicesSize, indices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(vectors::Vector3), (void*)0);
	glEnableVertexAttribArray(0);
}
