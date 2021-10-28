function loadmodule(code,name,chunkname)
	local f,error = load(code,chunkname)
	if f then 
		_G[name] = f()
	else
		print(error)
	end
end
