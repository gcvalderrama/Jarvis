Get-ChildItem ".\00Input\Input" -Filter *.txt | 
Foreach-Object {

    
    
    $fullname = $_.FullName


    $content = Get-Content $_.FullName       
    
    if ($content -like "*`*")
    {
        write-host $_.FullName
    }


    #filter and save content to the original file
    

    #filter and save content to a new file 
    #$content | Where-Object {$_ -match 'step[49]'} | Set-Content ($_.BaseName + '_out.log')
}