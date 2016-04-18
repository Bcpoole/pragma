build:
	mcs LexerScanner.cs Lexer.cs Lexeme.cs

run:
	make build
	mono LexerScanner.exe simple.txt
	mono LexerScanner.exe program.txt

clean:
	rm *.exe
