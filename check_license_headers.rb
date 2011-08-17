module FileBrowser
 def browse(root)
   queue = Array.new.push(root)
   while !queue.empty?
     filename = queue.pop
     if File.file?(filename)
       yield(filename)
     else
       Dir.new(filename).each do |child|
         unless ['..', '.','.svn'].include? child
           queue.push(filename + "/" + child)
         end
       end
     end
   end
 end
end

class HeadersCheck
 EXT = ['cs', 'bat']

 include FileBrowser

 def check_files(dir, dry_run)
   count = 0
   browse(dir) do |filename|
     if /\.#{EXT.join('$|\.')}$/ =~ filename
       match = nil
       f = File.new(filename)
       # Checking for the Apache header in the 4 first lines
       4.times do
         match ||= (/Copyright 2011 Manas/ =~
f.readline) rescue nil
           #puts("File #{filename} too short to check.")
       end
       f.close
       unless match
         if dry_run
           puts "Missing header in #{filename}"
         else
           add_header(filename)
         end
         count += 1
       end
     end
   end
   if dry_run
     puts "#{count} files don't have an Apache license header."
   else
     puts "#{count} files have been changed to include the Apache license
header."
   end
 end

 def add_header(filename)
   # Extracting file extension
   ext = /\.([^\.]*)$/.match(filename[1..-1])[1]
   header = HEADERS[ext]
   content = File.new(filename, 'r').read
   if content[0..4] == '<?xml'
     # If the file has a xml header, the license needs to be appended after
     content = content[0..content.index("\n")] + header + content[(
content.index("\n") + 1)..-1]
   else
     content = header + content
   end
   File.new(filename, 'w').write(content)
 end

end

CS_HEADER = <<CSHEADER
/**
 * gettext-cs-utils
 *
 * Copyright 2011 Manas Technology Solutions 
 * http://www.manas.com.ar/
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either 
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public 
 * License along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * 
 **/
CSHEADER

BAT_HEADER = <<BATHEADER
rem gettext-cs-utils
rem
rem Copyright 2011 Manas Technology Solutions 
rem http://www.manas.com.ar/
rem 
rem This library is free software; you can redistribute it and/or
rem modify it under the terms of the GNU Lesser General Public
rem License as published by the Free Software Foundation; either 
rem version 2.1 of the License, or (at your option) any later version.
rem 
rem This library is distributed in the hope that it will be useful,
rem but WITHOUT ANY WARRANTY; without even the implied warranty of
rem MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
rem Lesser General Public License for more details.
rem 
rem You should have received a copy of the GNU Lesser General Public 
rem License along with this library.  If not, see <http://www.gnu.org/licenses/>.

BATHEADER

HEADERS = {
 'cs' => CS_HEADER,
 'bat' => BAT_HEADER,
}

if ['-h', '--help', 'help'].include? ARGV[0]
 puts "Scans the current directory for files with missing license headers."
 puts "   ruby check_license_headers.rb      # list files"
 puts "   ruby check_license_headers.rb add  # add headers automatically"
else
 HeadersCheck.new.check_files('.', ARGV[0] != 'add')
end
