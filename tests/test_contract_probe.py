"""Syntax and documented API-shim behavior only; not a UE4SS/game compatibility test."""
import ctypes,ctypes.util,unittest
from pathlib import Path
SOURCE=(Path(__file__).resolve().parents[1]/'runtime/NativeContracts/Scripts/main.lua').read_bytes()
class ProbeTests(unittest.TestCase):
    def setUp(self):
        path=ctypes.util.find_library('lua5.4')
        if not path:self.skipTest('Lua 5.4 shared library unavailable')
        self.lua=ctypes.CDLL(path);l=self.lua
        l.luaL_newstate.restype=ctypes.c_void_p
        l.luaL_openlibs.argtypes=[ctypes.c_void_p];l.lua_close.argtypes=[ctypes.c_void_p]
        l.luaL_loadbufferx.argtypes=[ctypes.c_void_p,ctypes.c_char_p,ctypes.c_size_t,ctypes.c_char_p,ctypes.c_char_p]
        l.lua_pcallk.argtypes=[ctypes.c_void_p,ctypes.c_int,ctypes.c_int,ctypes.c_int,ctypes.c_longlong,ctypes.c_void_p]
        l.lua_tolstring.argtypes=[ctypes.c_void_p,ctypes.c_int,ctypes.c_void_p];l.lua_tolstring.restype=ctypes.c_char_p
        self.state=l.luaL_newstate();l.luaL_openlibs(self.state);self.addCleanup(l.lua_close,self.state)
    def run_lua(self,src):
        if isinstance(src,str):src=src.encode()
        rc=self.lua.luaL_loadbufferx(self.state,src,len(src),b'test',None)
        if not rc:rc=self.lua.lua_pcallk(self.state,0,0,0,0,None)
        self.assertEqual(rc,0,(self.lua.lua_tolstring(self.state,-1,None) or b'').decode(errors='replace'))
    def test_absent_loader_disables_cleanly(self):
        self.run_lua('logs={};print=function(v) table.insert(logs,v) end')
        self.run_lua(SOURCE)
        self.run_lua('assert(#logs==1 and string.find(logs[1],"DISABLED"))')
    def test_read_only_inventory_and_missing_classes(self):
        self.run_lua('''
logs={};print=function(v) table.insert(logs,v) end
Key={F8=119};queued=0;lookups=0
RegisterKeyBind=function(key,fn) assert(key==119);key_callback=fn end
ExecuteInGameThread=function(fn) queued=queued+1;fn() end
RegisterHook=function() error("No hook allowed") end
local invalid={IsValid=function() return false end}
local cls={IsValid=function() return true end,GetFullName=function() return "Class /Script/Dungeons.InventoryItem" end}
cls.ForEachFunction=function(self,cb) cb({GetFullName=function() return "Function /Script/Dungeons.InventoryItem:Example" end,GetFunctionFlags=function() return 1024 end}) end
cls.ForEachProperty=function(self,cb) cb({GetFullName=function() return "IntProperty /Script/Dungeons.InventoryItem:ExampleField" end}) end
cls.GetSuperStruct=function() return invalid end
StaticFindObject=function(path) lookups=lookups+1;if path=="/Script/Dungeons.InventoryItem" then return cls else return invalid end end
''')
        self.run_lua(SOURCE)
        self.run_lua('''assert(lookups==0);key_callback();assert(queued==1 and lookups==32)
local text=table.concat(logs);assert(string.find(text,"FUNCTION Function /Script/Dungeons.InventoryItem:Example flags=1024",1,true));assert(string.find(text,"PROPERTY IntProperty",1,true));assert(string.find(text,"MISSING /Script/Dungeons.UniqueCollectItem",1,true));assert(string.find(text,"END Names/flags only",1,true))''')
if __name__=='__main__':unittest.main()
