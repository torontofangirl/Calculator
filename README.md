# Calculator
A calculator with some CAS (Computer Algebra System) functionality.

## Eval
Evaluate a basic equation
```
$ dotnet run eval '5+2*3'
11

$ dotnet run eval '~2^4'
-16
```

Support for basic functions and constants
```
$ dotnet run eval 'max(cos(pi),cos(pi/2))'
0
```

Support for substituting variables
```
$ dotnet run eval 'x^4' x=~2
-16
```


## CAS (Computer Algebra System)
Simplify a polynomial
```
$ dotnet run cas 'x^2+2x^2-3x' simplify
3x^2-3x

$ dotnet run cas '(-3x+5)^4' binomialtheorem
81x^4-540x^3+1350x^2-1500x+625
```

Factorize a polynomial
```
$ dotnet run cas '3a^2+10a-1000' polyfactor a
(a+20)(3a-50)
```

Perform synthetic division on a polynomial
```
$ dotnet run cas 'x^2-5x+6' syntheticdiv 2 x
x-3 | remainder: 0
```

Use shoelace formula to calculate area of polygon given vertices
```
$ dotnet run cas (1,6)(3,-1)(-5,-3)(-4,3) shoelace
43.5
```

and more to come! (hopefully)


## Equation Solver (hillclimbing)
```
$ dotnet run hillclimb 'x^2-2x+1' x
1.0
```

## ROADMAP
* More CAS functions
