DIR="./DPLP/data"
for file in ./home/gregoryrst/Documents/jarvis/Lincoln/01DocumentExpansion/Output/*.txt
do
	# do something on $file
	if [ "$(ls -A $DIR)" ]; then
	    echo "Take action $DIR is not Empty"
	    rm -rf $DIR/*
	else
	    echo "$DIR is Empty"
	fi	

	filename=$(basename "$file")
	extension="${filename##*.}"
	filename="${filename%.*}"


	cp $file $DIR
	cd "DPLP"
	./corenlp.sh "./data"
	sleep 2
	python "./convert.py" "./data"
	sleep 2
	python "./segmenter.py" "./data"
	sleep 2
	python "./rstparser.py" "./data" false
	sleep 2

	cd ..
	fstart="./DPLP/data/$filename.txt.xml.jarvis"
	fend="./jarvis/Lincoln/03RST/Input/$filename.txt.xml.jarvis"
	cp $fstart $fend 
done





