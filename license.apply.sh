for i in *.cs
do
  if ! grep -q "Copyright 2011 Manas" $i
  then
    cat license.lgpl.header $i >$i.new && mv $i.new $i
  fi
done

