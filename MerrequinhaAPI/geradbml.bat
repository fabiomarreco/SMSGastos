call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86

sqlmetal /server:localhost /database:smsgastosdb /dbml:merrequinha.dbml /pluralize /language:csharp /user:sa

pause