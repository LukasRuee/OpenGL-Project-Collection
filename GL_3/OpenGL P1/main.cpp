#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <iostream>	
#include <string>

#include <glm/glm/glm.hpp>
#include <glm/glm/gtc/matrix_transform.hpp>
#include <glm/glm/gtc/type_ptr.hpp>
#include <helpers/shader.h>

#include <Utility/Utility.h>

struct Shape {
	std::string name;
	glm::vec3* vertices;
	size_t vertexCount;
	unsigned int* indices;
	unsigned int indicesSize;
};

// functions
void framebufferSizeCallback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow* window);
void CalculateTick();

void setTransform(Shader shader);
void bind(Shape shape);

// settings
static int SCREEN_WIDTH = 1600;
static int SCREEN_HEIGHT = 900;

static float MOVE_SPEED = 0.1f;
static float ROTATION_SPEED = 1.0f;
static float DEADZONE = 0.1f;

// time
double currentFrameTime = 0;
double lastFrameTime = 0;
double elapsedTime = 0;
double deltaTime = 0;

#pragma region Shapes

//Triangle
glm::vec3 vertices_triangle[] = {
	glm::vec3(0.0f,  0.0f, 0.0f),  // bottom left
	glm::vec3(1.0f,  0.0f, 0.0f),  // bottom right
	glm::vec3(0.0f,  1.0f, 0.0f)   // top center
};
unsigned int indices_triangle[] = {
	0, 1, 2
};

//Rectangle
glm::vec3 vertices_rectangle[] = {
	glm::vec3(-0.5f, -0.5f, 0.0f),  // bottom left
	glm::vec3(0.5f, -0.5f, 0.0f),   // bottom right
	glm::vec3(0.5f,  0.5f, 0.0f),    // top right
	glm::vec3(-0.5f,  0.5f, 0.0f)   // top left
};
unsigned int indices_rectangle[] = {
	0, 1, 2,
	2, 3, 0
};

//Cube
glm::vec3 vertices_cube[] = {
	// Bottom face
	glm::vec3(-0.5f, -0.5f, 0.5f),  // bottom left near
	glm::vec3(0.5f, -0.5f, 0.5f),   // bottom right near
	glm::vec3(0.5f,  0.5f, 0.5f),   // top right near
	glm::vec3(-0.5f,  0.5f, 0.5f),  // top left near

    // Top face
	glm::vec3(- 0.5f, -0.5f,  -0.5f),  // bottom left far
	glm::vec3(0.5f, -0.5f,  -0.5f),   // bottom right far
	glm::vec3(0.5f,  0.5f,  -0.5f),   // top right far
	glm::vec3(-0.5f,  0.5f,  -0.5f)   // top left far
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
glm::vec3 vertices_pyramid[] = {
	glm::vec3(-0.5f, -0.5f, 0.5f),  // bottom left near
	glm::vec3(0.5f, -0.5f, 0.5f),   // bottom right near
	glm::vec3(-0.5f, -0.5f,  -0.5f),  // bottom left far
	glm::vec3(0.5f, -0.5f,  -0.5f),   // bottom right far

	glm::vec3(0.0f,  0.5f, 0.0f)  // top center
};
unsigned int indices_pyramid[] = {
	0, 1, 2,	//Bottom A
	2, 3, 0,	//Bottom B
	0, 1, 4,	//Front
	1, 2, 4,	//Right
	2, 3, 4,	//Back
	3, 0, 4,	//Left
};

#pragma endregion

Shape shapes[] = {
	{"triangle", vertices_triangle, sizeof(vertices_triangle), indices_triangle, sizeof(indices_triangle)},
	{"rectangle", vertices_rectangle, sizeof(vertices_rectangle), indices_rectangle, sizeof(indices_rectangle)},
	{"cube", vertices_cube, sizeof(vertices_cube), indices_cube, sizeof(indices_cube)},
	{"pyramid", vertices_pyramid, sizeof(vertices_pyramid), indices_pyramid, sizeof(indices_pyramid)}
};

// shape array
const unsigned int shapeCount = sizeof(shapes) / sizeof(Shape);
unsigned short shapeIndex = 0;
bool recalculateShape = true;

unsigned int VAO, VBO, EBO; //Why only define inside main

	int main()
	{
		// Initialize and configure
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	
		// GLFW Window creation
		GLFWwindow* window = glfwCreateWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Super cool and epic OpenGL window", NULL, NULL);
		if (window == NULL)
		{
			std::cout << "Failed to create GLFW window" << std::endl;
			glfwTerminate();
			return -1;
		}
		glfwMakeContextCurrent(window);
		if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
		{
			std::cout << "Failed to initialize GLAD" << std::endl;
			return -1;
		}

		// Set up viewport resize callback (optional but useful)
		glfwSetFramebufferSizeCallback(window, framebufferSizeCallback);

		//Shader setup
		Shader shader("vert.vs", "frag.fs");
		shader.use();//Wraps the glUseProgram(shaderProgram) call

		//Draw mode settings
		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
		glEnable(GL_DEPTH_TEST);
		glfwSwapInterval(1); // Enable VSync for not crashing my pc

		//Bind Buffers
		glGenVertexArrays(1, &VAO);
		glGenBuffers(1, &VBO);
		glGenBuffers(1, &EBO);
		glBindVertexArray(VAO);

		//Shape
		int size;
		shapes[shapeIndex];
		bind(shapes[shapeIndex]);
		glGetBufferParameteriv(GL_ELEMENT_ARRAY_BUFFER, GL_BUFFER_SIZE, &size);//Call after binding!!!

		//Redraw frame
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);

		while (!glfwWindowShouldClose(window))
		{
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			CalculateTick();
			processInput(window);

			if (recalculateShape)
			{
				recalculateShape = false;
				Shape& shape = shapes[shapeIndex];
				bind(shape);
				std::cout << "Shape name: " << shape.name << std::endl;
				std::cout << "Switched to shape index: " << shapeIndex << std::endl;
				glGetBufferParameteriv(GL_ELEMENT_ARRAY_BUFFER, GL_BUFFER_SIZE, &size);
			}

			// Set transforms and draw
			setTransform(shader);

			//glDrawElements(GL_TRIANGLES, shape.indicesSize, GL_UNSIGNED_INT, (void*)0);
			glDrawElements(GL_TRIANGLES, size / sizeof(GLuint), GL_UNSIGNED_INT, (void*)0);

			glfwSwapBuffers(window);
			glfwPollEvents();
		}

	glfwTerminate();
	return 0;
}

void CalculateTick()
{
	const double currentFrameTime = glfwGetTime();
	deltaTime = currentFrameTime - lastFrameTime;
	lastFrameTime = currentFrameTime;
	elapsedTime += deltaTime;
}

static bool upKeyPressed = false;
static bool spaceKeyPressed = false;
static bool downKeyPressed = false;
static bool leftShiftKeyPressed = false;

static bool gButtonDpadUpPressed = false;
static bool gButtonDpadDownPressed = false;
static bool gButtonGuidePressed = false;
static bool gButtonRighBumperPressed = false;

static bool manualControl = false;

// Axis values for both joysticks
float leftStickX = 0.0f;
float leftStickY = 0.0f;
float rightStickX = 0.0f;
float rightStickY = 0.0f;

void processInput(GLFWwindow* window)
{
	// Toggle manual mode
	if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS && !spaceKeyPressed)
	{
		spaceKeyPressed = true;
		manualControl = !manualControl;
	}
	if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_RELEASE)
	{
		spaceKeyPressed = false;
	}

	// Close window
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
	{
		glfwSetWindowShouldClose(window, true);
	}

	// Wireframe options
	if (glfwGetKey(window, GLFW_KEY_1) == GLFW_PRESS)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}
	if (glfwGetKey(window, GLFW_KEY_2) == GLFW_PRESS)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
	}
	if (glfwGetKey(window, GLFW_KEY_3) == GLFW_PRESS)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_POINT);
	}

	// Scrolling trough meshes
	if (glfwGetKey(window, GLFW_KEY_UP) == GLFW_PRESS && !upKeyPressed)
	{
		upKeyPressed = true;
		shapeIndex = (shapeIndex + 1) % shapeCount;
		recalculateShape = true;
	}
	if (glfwGetKey(window, GLFW_KEY_UP) == GLFW_RELEASE)
	{
		upKeyPressed = false;
	}
	if (glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_PRESS && !downKeyPressed)
	{
		downKeyPressed = true;
		shapeIndex = (shapeIndex - 1) % shapeCount;
		recalculateShape = true;
	}
	if (glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_RELEASE)
	{
		downKeyPressed = false;
	}

	//Zoom
	if (glfwGetKey(window, GLFW_KEY_LEFT_SHIFT) == GLFW_PRESS && !leftShiftKeyPressed)
	{
		leftShiftKeyPressed = true;
	}
	if (glfwGetKey(window, GLFW_KEY_LEFT_SHIFT) == GLFW_RELEASE)
	{
		leftShiftKeyPressed = false;
	}

	//Location
	leftStickX += glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS ? -1 :
	glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS ? 1 : 0;

	leftStickY += glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS ? -1 :
		glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS ? 1 : 0;
	//Rotatiom
	rightStickX += glfwGetKey(window, GLFW_KEY_Q) == GLFW_PRESS ? -1 :
		glfwGetKey(window, GLFW_KEY_E) == GLFW_PRESS ? 1 : 0;

	rightStickY += glfwGetKey(window, GLFW_KEY_F) == GLFW_PRESS ? -1 :
		glfwGetKey(window, GLFW_KEY_R) == GLFW_PRESS ? 1 : 0;


	// Controller support
	if (glfwJoystickIsGamepad(GLFW_JOYSTICK_1))
	{
		GLFWgamepadstate state;
		if (glfwGetGamepadState(GLFW_JOYSTICK_1, &state))
		{
			// Toggle manual mode
			if (state.buttons[GLFW_GAMEPAD_BUTTON_START] == GLFW_PRESS && !gButtonGuidePressed)
			{
				gButtonGuidePressed = true;
				manualControl = !manualControl;
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_START] == GLFW_RELEASE)
			{
				gButtonGuidePressed = false;
			}

			// Close window
			if (state.buttons[GLFW_GAMEPAD_BUTTON_BACK] == GLFW_PRESS)
			{
				glfwSetWindowShouldClose(window, true);
			}

			// Wire frame selection
			if (state.buttons[GLFW_GAMEPAD_BUTTON_A] == GLFW_PRESS)
			{
				glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_B] == GLFW_PRESS)
			{
				glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_X] == GLFW_PRESS)
			{
				glPolygonMode(GL_FRONT_AND_BACK, GL_POINT);
			}

			// Scrolling trough meshes
			if (state.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP] == GLFW_PRESS && !gButtonDpadUpPressed)
			{
				gButtonDpadUpPressed = true;
				shapeIndex = (shapeIndex + 1) % shapeCount;
				recalculateShape = true;
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP] == GLFW_RELEASE)
			{
				gButtonDpadUpPressed = false;
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN] == GLFW_PRESS && !gButtonDpadDownPressed)
			{
				gButtonDpadDownPressed = true;
				shapeIndex = (shapeIndex + 1) % shapeCount;
				recalculateShape = true;
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN] == GLFW_RELEASE)
			{
				gButtonDpadDownPressed = false;
			}

			// Zoom
			if (state.buttons[GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER] == GLFW_PRESS && !gButtonRighBumperPressed)
			{
				gButtonRighBumperPressed = true;
			}
			if (state.buttons[GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER] == GLFW_RELEASE)
			{
				gButtonRighBumperPressed = false;
			}

			// Axis (range: -1.0 to 1.0)
			//Location
			if (fabs(state.axes[GLFW_GAMEPAD_AXIS_LEFT_X]) > DEADZONE)
				leftStickX += state.axes[GLFW_GAMEPAD_AXIS_LEFT_X];
			if (fabs(state.axes[GLFW_GAMEPAD_AXIS_LEFT_Y]) > DEADZONE)
				leftStickY += state.axes[GLFW_GAMEPAD_AXIS_LEFT_Y] * -1;

			//Rotation
			if (fabs(state.axes[GLFW_GAMEPAD_AXIS_RIGHT_X]) > DEADZONE)
				rightStickX += state.axes[GLFW_GAMEPAD_AXIS_RIGHT_X];
			if (fabs(state.axes[GLFW_GAMEPAD_AXIS_RIGHT_Y]) > DEADZONE)
				rightStickY += state.axes[GLFW_GAMEPAD_AXIS_RIGHT_Y] * -1;
		}
	}
}

void framebufferSizeCallback(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
}


void setTransform(Shader shader)
{
	const float aspect = static_cast<float>(SCREEN_WIDTH) / static_cast<float>(SCREEN_HEIGHT);
	float fov = 90.0f;

	//Matrices
	glm::mat4 model = glm::mat4(1.0f);
	glm::mat4 view = glm::mat4(1.0f);
	glm::mat4 projection = glm::mat4(1.0f);

	glm::mat4 t, r, s;
	s = glm::scale(glm::mat4(1.0f), glm::vec3(1.0f));

	if (manualControl)
	{
		// Manual movement (controller or keyboard)
		t = glm::translate(glm::mat4(1.0f), glm::vec3(leftStickX * MOVE_SPEED, leftStickY * MOVE_SPEED, 3.0f));

		const glm::mat4 rotX = glm::rotate(glm::mat4(1.0f), glm::radians(rightStickY * ROTATION_SPEED), glm::vec3(1, 0, 0));
		const glm::mat4 rotY = glm::rotate(glm::mat4(1.0f), glm::radians(rightStickX * ROTATION_SPEED), glm::vec3(0, 1, 0));
		r = rotY * rotX;
		fov = (leftShiftKeyPressed || gButtonRighBumperPressed) ? 45.0f : 90.0f;
	}
	else
	{
		// Automatic movement
		float autoX = glm::mix(-1.0f, 1.0f, Utility::sinPos(elapsedTime * 0.5f));
		t = glm::translate(glm::mat4(1.0f), glm::vec3(autoX, 0.0f, 3.0f));
		r = glm::rotate(model, glm::radians(static_cast<float>(elapsedTime * 45.0)), glm::vec3(1, 1, 1));

		fov = glm::mix(45.0f, 90.0f, Utility::sinPos(elapsedTime) * 0.5f); //mix == slerp
	}
	model = t * r * s;

	view = glm::translate(view, glm::vec3(0.0f, 0.0f, -10.0f));
	projection = glm::perspective(glm::radians(fov), aspect, 0.1f, 100.0f);

	glUniformMatrix4fv(glGetUniformLocation(shader.ID, "model"), 1, GL_FALSE, glm::value_ptr(model));
	glUniformMatrix4fv(glGetUniformLocation(shader.ID, "view"), 1, GL_FALSE, glm::value_ptr(view));
	shader.setMat4("projection", projection);
}
/*
void setTransform(Shader shader)
{
	const float aspect = static_cast<float>(SCREEN_WIDTH) / static_cast<float>(SCREEN_HEIGHT);
	const float aspectRatio = (float)SCREEN_WIDTH / (float)SCREEN_HEIGHT;

	//Matrices
	glm::mat4 model = glm::mat4(1.0f);
	glm::mat4 view = glm::mat4(1.0f);
	glm::mat4 projection = glm::mat4(1.0f);

	// Model transformation: rotate, translate, scale
	const float x = glm::mix(-1.0f, 1.0f, Utility::sinPos(elapsedTime * 0.5f));

	//const glm::mat4 t = glm::translate(model, glm::vec3(x, 0.0f, 3.0f)); // Move right and forward
	const glm::mat4 t = glm::translate(model, glm::vec3(leftStickX * MOVE_SPEED, leftStickY * MOVE_SPEED, 3.0f));
	const glm::mat4 rotX = glm::rotate(glm::mat4(1.0f), glm::radians(rightStickY * ROTATION_SPEED), glm::vec3(1, 0, 0));
	const glm::mat4 rotY = glm::rotate(glm::mat4(1.0f), glm::radians(rightStickX * ROTATION_SPEED), glm::vec3(0, 1, 0));
	const glm::mat4 r = rotY * rotX; // Order matters (Y * X for yaw-then-pitch)
	//const glm::mat4 r = glm::rotate(model, glm::radians(45.0f), glm::vec3(1, 1, 0));
	const glm::mat4 s = glm::scale(model, glm::vec3(1.0f));
	model = t * r * s;

	// View transformation: camera moves back
	view = glm::translate(view, glm::vec3(0.0f, 0.0f, -10.0f));

	// Perspective Projectionfloat 
	//projection = glm::ortho(glm::radians(60.0f), aspect, 0.1f, 100.0f)
	projection = glm::perspective(glm::radians(45.0f), aspect, 0.1f, 100.0f);

	//const float fov = glm::mix(45.0f, 90.0f, Utility::sinPos(elapsedTime) * 0.5f); //mix == slerp
	const float fov = (lmbMousePressed || gButtonRighBumperPressed) ? 45.0f : 90.0f;

	projection = glm::perspective(glm::radians(fov), aspect, 0.1f, 100.0f);	//Assigning directly also works
	//model = glm::rotate(model, glm::radians((float)(elapsedTime * 45.0)), glm::vec3(1, 1, 1));
	 
	//Rotation calc will be made directly above
	//model = glm::rotate(model, glm::radians((float)(rightStickY * 1.0)), glm::vec3(1, 0, 0));
	//model = glm::rotate(model, glm::radians((float)(rightStickX * 1.0)), glm::vec3(0, 1, 0));

	//model = glm::translate(model, glm::vec3(0,(float)(leftStickY * 0.01f),0));
	//model = glm::translate(model, glm::vec3((float)(leftStickX * 0.01f),0 ,0));

	// Send to shaders
	glUniformMatrix4fv(glGetUniformLocation(shader.ID, "model"), 1, GL_FALSE, glm::value_ptr(model));
	glUniformMatrix4fv(glGetUniformLocation(shader.ID, "view"), 1, GL_FALSE, glm::value_ptr(view));
	shader.setMat4("projection", projection);
}*/

void bind(Shape shape)
{
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, shape.vertexCount * sizeof(glm::vec3), shape.vertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, shape.indicesSize, shape.indices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), (void*)0);
	glEnableVertexAttribArray(0);
	glBindVertexArray(VAO);

}