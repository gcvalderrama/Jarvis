AMRDIR="../camr/AMRParsing"
JARVISDIR="../../Jarvis/Lincoln/05AMRParsing/Output"
for file in `ls ./Lincoln/01DocumentExpansion/Output/*.txt | sort -V`; do 
	if [ "$(ls -A $AMRDIR)" ]; then
	    rm -rf $AMRDIR/*.txt
	    rm -rf $AMRDIR/*.txt.tok
	    rm -rf $AMRDIR/*.txt.tok.charniak.parse
	    rm -rf $AMRDIR/*.txt.tok.charniak.parse.dep
	    rm -rf $AMRDIR/*.txt.prp
	    rm -rf $AMRDIR/*.txt.all.basic-abt-brown-verb.parsed
	    rm -rf $AMRDIR/log/*
	fi	
        #rm -rf $DIR/*
	filename=$(basename "$file")
	extension="${filename##*.}"
	filename="${filename%.*}"
	echo $AMRDIR
	cp $file $AMRDIR
	cd ..
	cd "camr"
	cd AMRParsing


	dos2unix "$filename.txt"

	
	FILESIZE=1
	counter=$1
	while [ $FILESIZE -gt 0 ]
	do	  
	  rm -rf *.txt.tok
	  rm -rf *.txt.tok.charniak.parse
	  rm -rf *.txt.tok.charniak.parse.dep
	  rm -rf *.txt.prp
	  rm -rf *.txt.all.basic-abt-brown-verb.parsed
	  rm -rf ./log/*
		
	  python "amr_parsing.py" -m preprocess "$filename.txt" 2>log/"process$filename.log"
  	  sleep 2s
	  python "amr_parsing.py" -m parse --model "amr-anno-1.0.train.basic-abt-brown-verb.m" "$filename.txt" 	2>log/"parse$filename.log"
	  FILESIZE=$(stat -c%s "./log/parse$filename.log")
	  counter=$(( $counter + 1 ))
	  echo "Parse error on $filename more than 0 in $counter File size : $FILESIZE "
	done

	sleep 2s
	unix2dos "$filename.txt.all.basic-abt-brown-verb.parsed"
	cp "$filename.txt.all.basic-abt-brown-verb.parsed" $JARVISDIR
	cd ..
	cd ..
	cd Jarvis
#	fstart="./DPLP/data/$filename.txt.xml.jarvis"
#	fend="./jarvis/Lincoln/03RST/Input/$filename.txt.xml.jarvis"
#	cp $fstart $fend 
done
