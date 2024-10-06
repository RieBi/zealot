# Zealot
DEFinitive interpreted programming language I've made from scratch, implementing the basic features of a programming language. Can be run both interactively on the go, and read from a file.

# Demo
![ZEALOTDEMO](https://github.com/user-attachments/assets/502fce36-1fce-4b0c-bc84-13d97d698744)


# Motivation
The goal of the project was to deepen my understanding of programming languages, how they work, understand how lexing, parsing, scoping, running the language actually works. Building it myself is indeed the best way to learn it.

# Feature showcase
```zealot
printn("Hello, world!")

def num = 1
def precision_num = 2.2e4
def some_string = "My lovely string"

define add =>
	def a = 2
	def b = 3
	a + b

define multiply(a, b) =>
	a * b

define calculate(a, b) =>
	(a + (b * 2) / 2 $ 3 $ a) % (12 * b + 7)

def result1 = multiply(2, 3)

def var1 = 2
def var2 = 3
def result2 = calculate(var1, var2)

repeat(10) =>
	printn("Hello")

def i = 0
repeat(i < 10) =>
	i += 1
	printn(i)

repeat(def j = 0, def k = 2 ? j < 10 && k < 20 : j += 1, k += 4, printn(k)) =>
	printn(j)

def bool_val1 = false
def bool_val2 = bool_val1 || true && true
if (bool_val2) =>
	printn("True")
elseif (2 > 3) =>
	printn("2 is greater than 3")
else =>
	printn("Neither is true")

define fib(n) =>
	def result = 0
	if (n == 0 || n == 1) =>
			result = 1
	else =>
			result = fib(n - 1) + fib(n - 2)
	result

printn(fib(5))
printn(fib(8))
```

# Usage Guide
Dotnet 8 SDK and runtime is needed to run the app locally, which can be downloaded at: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

1. Clone the repository:
   ```
   git clone https://github.com/RieBi/zealot.git
   ```
1. Navigate to the project directory:
   ```
   cd zealot
   ```
1. Run the project:
   ```
   dotnet run --project zealot.runner
   ```
   
The Zealot would then run in interactive mode, where you can type the code, and see the execution results in real time.
