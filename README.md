# Interpreted-Language
An interpeted language, originally written by tylerlaceby (https://youtube.com/playlist?list=PL_2VhOvlMk4UHGqYCLWc6GO8FaPl8fQTh&amp;si=VDTC6ko_LgYYFda-) in typescript. This is rewritten in C# and has added some basic support for the following: 
strings ("" and '' strings!), comments (#, python style), unary operators (i++ and i-- currently), boolean expressions, if/ifel/else statements, and a really hacked together Exit function!
(i also renamed fn to func because i didn't like that.)

I plan to add a few more things at least, and then maybe use this in a game or something like Sebastian Lague did here (https://youtu.be/dY6jR52fFWo?si=bWCFwUMwjVPhTcBw)!

plans, in no particular order:
"using" keyword. work similar to C#, but instead would take a file path and then import the contents of that file and lex it all first. should be prettttty easy (says having not done it yet)

"class" keyword. i have ideas on how to add some C# style "classes", similar to the object. ideally theyll have a type (probably a string) and that kind of thing. will be fun!

"for"/"while" keywords. Loops! I love a loop! itll be a bit goofy but loops are great.
(and maybe "do"?)

Arrays/Lists/Maps: Storing / Accessing arrays would be really cool!

Some more native functions: Rounding and some other math functions would be nice, among other things.
Having them written in C# theoretically means they're faster! WARNING: Have not run actual benchmark tests. yet.

Easier ways to add native functions: the way its done at the moment uhhhhhhh kinda sucks. i wanna be able to do it easier, and while id prefer it happening automatically it might have to be something done through attributes and headers and that sounds prettty painful. But it would make this super portable to other C# projects that wanna use this language! for some reason.

Chars/Ints?: this is a big ol maybe. because i like both ints AND chars, but i dont like the idea of dealing with them. So maybe.

and of course, I want to finish unary operators (++i, --i, **i, etc are not yet implemented) and any other bugs that exist.
