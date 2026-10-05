"""Read observed Lovika JSON with comments/trailing commas without altering strings."""
import json
from pathlib import Path
def load(p):
 s=Path(p).read_text(encoding='utf-8-sig');out=[];quote=False;escape=False;i=0
 while i<len(s):
  c=s[i]
  if quote:
   out.append(c)
   if escape: escape=False
   elif c=='\\':escape=True
   elif c=='"':quote=False
   i+=1;continue
  if c=='"':quote=True;out.append(c);i+=1;continue
  if s[i:i+2]=='//':
   i=s.find('\n',i)
   if i<0:break
   continue
  if s[i:i+2]=='/*':
   end=s.find('*/',i+2)
   if end<0:raise ValueError('Unterminated comment')
   i=end+2;continue
  if c==',':
   k=i+1
   while k<len(s) and s[k].isspace():k+=1
   if k<len(s) and s[k] in '}]':i+=1;continue
  out.append(c);i+=1
 return json.loads(''.join(out))
