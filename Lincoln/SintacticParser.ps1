param (
    $inputDir = 'D:\Tesis2016\Jarvis\Lincoln\00Input\Input'    
)


$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$stdcore = "D:\Tesis2016\CORENLP\stanford-corenlp-full-2015-12-09\*"
$Output00Dir = 'D:\Tesis2016\Jarvis\Lincoln\00Input\Output'
$java= "java.exe"

Get-ChildItem $Output00Dir -Filter *.xml | 
Foreach-Object {
    Remove-Item $_.FullName        
}

Get-ChildItem $inputDir -Filter *.txt | 
Foreach-Object {
    $filename = $_.FullName    
    & $java -cp $stdcore -Xmx2g edu.stanford.nlp.pipeline.StanfordCoreNLP -annotators tokenize,ssplit,pos,lemma,ner,parse,dcoref -file $filename -outputFormat xml  -outputDirectory $Output00Dir -replaceExtension        
}


&$msbuild  "D:\Tesis2016\Jarvis\Lincoln\01DocumentExpansion\Jarvis\Jarvis.sln" /verbosity:q /p:configuration=Release /t:Clean,Build

$Output01Dir = 'D:\Tesis2016\Jarvis\Lincoln\01DocumentExpansion\Output'

Get-ChildItem $Output01Dir -Filter *.txt | 
    Foreach-Object {
        Remove-Item $_.FullName        
}

& "D:\Tesis2016\Jarvis\Lincoln\01DocumentExpansion\Jarvis\Jarvis\bin\release\Jarvis.exe" -i $Output00Dir -o $Output01Dir 
#-d


$OutputSintactic = 'D:\Tesis2016\Jarvis\Lincoln\02SintacticAnalysis\Output'

Get-ChildItem $OutputSintactic -Filter *.xml | 
Foreach-Object {
    Remove-Item $_.FullName        
}

Get-ChildItem $Output01Dir -Filter *.txt | 
Foreach-Object {
    $filename = $_.FullName
    & $java -cp $stdcore -Xmx2g edu.stanford.nlp.pipeline.StanfordCoreNLP -annotators tokenize,ssplit,pos,lemma,ner,parse,dcoref -file $filename -outputFormat xml  -outputDirectory $OutputSintactic -replaceExtension        
}



#-cp "D:\Tesis2016\CORENLP\stanford-corenlp-full-2015-12-09\*" -Xmx2g
 #Start-Process java -ArgumentList '-version' -RedirectStandardOutput '.\console.out' -RedirectStandardError '.\console.err' 
 
 
  