build:
	mcs RecognizerScanner.cs Recognizer.cs Lexer.cs Lexeme.cs Node.cs

run:
	make build
	mono RecognizerScanner.exe simple.txt
	mono RecognizerScanner.exe program.txt

clean:
	rm *.exe
