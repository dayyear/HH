<?php

if (count($_FILES)<1){
    die('Err:format');
}

$file = $_FILES['image'];

$file = new File($file);

$max_size = 102400;
$type = 'image';
$path = "/";


if ($file->type != $type){
    die('Err:format');
}

if ($file->size()>$max_size){
    die('Err:big');
}

$result = $file->copyFromTemp();

if ($result){
    die($result);
}

die('Err:format');


class File{
    var $name = null;
    var $temp_name = null;
    var $extension = null;
    var $type = null;
    var $size = null;

    function File($file = null){

	# $file is an entry from $_FILES

	if ($file != null){
	    $this->name = $file['name'];
	    $this->temp_name = $file['tmp_name'];
	    $this->size = $file['size'];

	    $type = explode('/',$file['type']);
	    $this->type = $type[0];

	    $this->extension = substr($this->name,strrpos($this->name, ".")+1);
	}
	else {
	    return null;
	}
    }

    function name(){
	return $this->name;
    }
    function type(){
	return $this->type;
    }
    function size(){
	return $this->size;
    }
    function tmp_name(){
	return $this->temp_name;
    }
    function extension(){
	return $this->extension;
    }

    function randomName(){
	return md5(rand()) . "." .$this->extension;
    }

    function copyFromTemp($path = "",$keep_name = false){

	# copies file from tmp_file to given path
	# returns new file name if success
	# returns false if failure
	# will try to find a random name which is not taken in this path

	# if $keep_name is true, will try to save under the current $name or return false

	$nname = $this->randomName();
	if ($keep_name){
	    $nname = $this->name;
	}
	else {
	    while (is_file($path . $nname)){
		$nname = $this->randomName();
	    }
	}
	if (is_file($path . $nname)){
		return false;
	}

	if (move_uploaded_file($this->temp_name,$nname)){
	    return $nname;
	}

	return false;

    }
}
?>