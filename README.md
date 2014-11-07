# BooleanCombination

### What is this?

BooleanCombination outputs a function that will match the inputted result.

  1. Input the result for the function
  2. Get the function
  
So simple.


### Why make this?

I have an on-going project now where I faced a situation where I wanted the method to run only when two specific booleans. I didn't want the code to be long and ugly so I thought about using bit operations to solve this problem. (I found a formula for that method by hand though)

So here is the problem I had: (C#)

``` C#
public bool runSetup = false;
public static void Setup(bool forcererun = false)
        {
            //Don't run when runSetup = true and forcererun = false
            //check if matches requirement above
            
            //some other work
        }

```
What I wanted: 

| runSetup   | forcererun  | expected result|
| -----------|-------------| -------------- |
| true       |true         | false          |
| true       |false        | false          |
| false      |true         | true           |
| false      |false        | false          |
  

I managed to hand come up with this function:

(runSetup & forcererun ^ runSetup)

### Other Inputs

Of course we want flexibility. There are two extra commands allowed instead of four-consecutive binary input.

 - _setop_
  * You can set which operators you want to use want and don't want to use. For example, you want all bit operators except for NOT. Input 'setop' and you can set your preferences. 
  * Special cases: 
     * Where A:1010 and B: 0110, we want the result to be 1010, which is exactly same as A.There are three answers to this (AFAIK). 
          *            (A)
          *            (NOT (NOT A)) 
          *            ((A AND B) OR A) 
     * Double-NOTs are sometimes useless (happened to me) so there is also an option to allow/block double-NOTs right after setting operators
 - _setab_
    - You may want to re-order the combinations - yes you can. Just type four-consecutive binaries twice to change set-A and set-B.

### Version

Yeah yeah kinda whatever. 
Ver 1.01

### How to run

Well, its a console application.
Using Developer Command Prompt and csc.exe would probably be the fastest + easiest

```
chdir [wherever you put Program.cs]
csc Program.cs
```
This will spit out an exe file named Program.exe
Rename it if you would. 

You can also do:
```
csc /out:BooleanCombination.exe Program.cs
```
which does the work.

### Framework

It's running .NET 4.5! Awesome!

### Development

I guess its finished as of now.

License
----
MIT

**Learn and utilize it if you have the chance**
