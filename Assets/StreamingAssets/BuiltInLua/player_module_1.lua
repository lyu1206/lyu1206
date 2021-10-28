local player = {}

local idx = 1
local updates = {}

function player.Update()
	for key, co in pairs(updates) do
	end
end
function player.RegistRoutine(name,code,chunk)
	local f,errmsg = load(code,chunk)
	if f == nil then		
		print(errmsg)
	end
	local co = coroutine.create(f)
	coroutine.resume(co)
	updates[idx] = co
	idx = idx + 1
end

Time = { deltaTime = 0 }



return player
