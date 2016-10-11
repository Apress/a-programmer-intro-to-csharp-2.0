csc /target:library logdriver.cs 
csc /r:logdriver.dll test.cs
csc /r:logdriver.dll /target:library LogAddInToFile.cs