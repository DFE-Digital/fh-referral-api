· 
eC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\ConfigureServices.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
;/ 0
public 
static 
class 
ConfigureServices %
{ 
public 

static 
IServiceCollection $%
AddInfrastructureServices% >
(> ?
this? C
IServiceCollectionD V
servicesW _
,_ `
IConfigurationa o
configurationp }
)} ~
{ 
services 
. 
AddTransient 
< "
IDomainEventDispatcher 4
,4 5!
DomainEventDispatcher6 K
>K L
(L M
)M N
;N O
services 
. 
	AddScoped 
< 1
%AuditableEntitySaveChangesInterceptor @
>@ A
(A B
)B C
;C D
var 
	useDbType 
= 
configuration %
.% &
GetValue& .
<. /
string/ 5
>5 6
(6 7
$str7 B
)B C
;C D
switch 
( 
	useDbType 
) 
{ 	
case 
$str &
:& '
services 
. 
AddDbContext %
<% & 
ApplicationDbContext& :
>: ;
(; <
options< C
=>D F
options 
. 
UseInMemoryDatabase +
(+ ,
$str, 8
)8 9
)9 :
;: ;
break 
; 
case 
$str '
:' (
services 
. 
AddDbContext %
<% & 
ApplicationDbContext& :
>: ;
(; <
options< C
=>D F
options   
.   
UseSqlServer   $
(  $ %
configuration  % 2
.  2 3
GetConnectionString  3 F
(  F G
$str  G [
)  [ \
??  ] _
String  ` f
.  f g
Empty  g l
)  l m
)  m n
;  n o
break!! 
;!! 
case## 
$str## &
:##& '
services$$ 
.$$ 
AddDbContext$$ %
<$$% & 
ApplicationDbContext$$& :
>$$: ;
($$; <
options$$< C
=>$$D F
options%% 
.%% 
	UseNpgsql%% !
(%%! "
configuration%%" /
.%%/ 0
GetConnectionString%%0 C
(%%C D
$str%%D X
)%%X Y
??%%Z \
String%%] c
.%%c d
Empty%%d i
)%%i j
)%%j k
;%%k l
break&& 
;&& 
default(( 
:(( 
services)) 
.)) 
AddDbContext)) %
<))% & 
ApplicationDbContext))& :
>)): ;
()); <
options))< C
=>))D F
options** 
.** 
UseInMemoryDatabase** /
(**/ 0
$str**0 <
)**< =
)**= >
;**> ?
break++ 
;++ 
},, 	
services.. 
... 
	AddScoped.. 
<.. !
IApplicationDbContext.. 0
>..0 1
(..1 2
provider..2 :
=>..; =
provider..> F
...F G
GetRequiredService..G Y
<..Y Z 
ApplicationDbContext..Z n
>..n o
(..o p
)..p q
)..q r
;..r s
services00 
.00 
	AddScoped00 
<00 +
ApplicationDbContextInitialiser00 :
>00: ;
(00; <
)00< =
;00= >
services22 
.22 
AddTransient22 
<22 
	IDateTime22 '
,22' (
DateTimeService22) 8
>228 9
(229 :
)22: ;
;22; <
return44 
services44 
;44 
}55 
}66 µ
|C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Config\ReferralConfiguration.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <
Config< B
;B C
public 
class !
ReferralConfiguration "
:# $$
IEntityTypeConfiguration% =
<= >
Referral> F
>F G
{ 
public		 

void		 
	Configure		 
(		 
EntityTypeBuilder		 +
<		+ ,
Referral		, 4
>		4 5
builder		6 =
)		= >
{

 
builder 
. 
Property 
( 
t 
=> 
t 
.  
FullName  (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> 
t 
.  
	ServiceId  )
)) *
. 
HasMaxLength 
( 
$num 
) 
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> 
t 
.  
ServiceName  +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> 
t 
.  
Created  '
)' (
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> 
t 
.  
	CreatedBy  )
)) *
. 
HasMaxLength 
( 
$num 
) 
. 

IsRequired 
( 
) 
; 
} 
} ó3
íC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Interceptors\AuditableEntitySaveChangesInterceptor.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Infrastructure		! /
.		/ 0
Persistence		0 ;
.		; <
Interceptors		< H
;		H I
public 
class 1
%AuditableEntitySaveChangesInterceptor 2
:3 4"
SaveChangesInterceptor5 K
{ 
private 
readonly 
ICurrentUserService (
_currentUserService) <
;< =
private 
readonly 
	IDateTime 
	_dateTime (
;( )
public 
1
%AuditableEntitySaveChangesInterceptor 0
(0 1
ICurrentUserService 
currentUserService .
,. /
	IDateTime 
dateTime 
) 
{ 
_currentUserService 
= 
currentUserService 0
;0 1
	_dateTime 
= 
dateTime 
; 
} 
public 

override 
InterceptionResult &
<& '
int' *
>* +
SavingChanges, 9
(9 :
DbContextEventData: L
	eventDataM V
,V W
InterceptionResultX j
<j k
intk n
>n o
resultp v
)v w
{ 
UpdateEntities 
( 
	eventData  
.  !
Context! (
)( )
;) *
return 
base 
. 
SavingChanges !
(! "
	eventData" +
,+ ,
result- 3
)3 4
;4 5
} 
public 

override 
	ValueTask 
< 
InterceptionResult 0
<0 1
int1 4
>4 5
>5 6
SavingChangesAsync7 I
(I J
DbContextEventDataJ \
	eventData] f
,f g
InterceptionResulth z
<z {
int{ ~
>~ 
result
Ä Ü
,
Ü á
CancellationToken
à ô
cancellationToken
ö ´
=
¨ ≠
default
Æ µ
)
µ ∂
{   
UpdateEntities!! 
(!! 
	eventData!!  
.!!  !
Context!!! (
)!!( )
;!!) *
return## 
base## 
.## 
SavingChangesAsync## &
(##& '
	eventData##' 0
,##0 1
result##2 8
,##8 9
cancellationToken##: K
)##K L
;##L M
}$$ 
public&& 

void&& 
UpdateEntities&& 
(&& 
	DbContext&& (
?&&( )
context&&* 1
)&&1 2
{'' 
if(( 

((( 
context(( 
==(( 
null(( 
)(( 
return(( #
;((# $
foreach** 
(** 
var** 
entry** 
in** 
context** %
.**% &
ChangeTracker**& 3
.**3 4
Entries**4 ;
<**; <

EntityBase**< F
<**F G
string**G M
>**M N
>**N O
(**O P
)**P Q
)**Q R
{++ 	
if,, 
(,, 
entry,, 
.,, 
State,, 
==,, 
EntityState,, *
.,,* +
Added,,+ 0
),,0 1
{-- 
if.. 
(.. 
entry.. 
... 
Entity..  
...  !
	CreatedBy..! *
==..+ -
null... 2
)..2 3
{// 
if00 
(00 
_currentUserService00 +
.00+ ,
UserId00, 2
!=003 5
null006 :
)00: ;
entry11 
.11 
Entity11 $
.11$ %
	CreatedBy11% .
=11/ 0
_currentUserService111 D
.11D E
UserId11E K
;11K L
else22 
entry33 
.33 
Entity33 $
.33$ %
	CreatedBy33% .
=33/ 0
$str331 9
;339 :
}55 
entry66 
.66 
Entity66 
.66 
Created66 $
=66% &
	_dateTime66' 0
.660 1
Now661 4
;664 5
}77 
if99 
(99 
entry99 
.99 
State99 
==99 
EntityState99 *
.99* +
Added99+ 0
||991 3
entry994 9
.999 :
State99: ?
==99@ B
EntityState99C N
.99N O
Modified99O W
||99X Z
entry99[ `
.99` a#
HasChangedOwnedEntities99a x
(99x y
)99y z
)99z {
{:: 
entry;; 
.;; 
Entity;; 
.;; 
LastModifiedBy;; +
=;;, -
_currentUserService;;. A
.;;A B
UserId;;B H
;;;H I
entry<< 
.<< 
Entity<< 
.<< 
LastModified<< )
=<<* +
	_dateTime<<, 5
.<<5 6
Now<<6 9
;<<9 :
}== 
}>> 	
}?? 
}@@ 
publicBB 
staticBB 
classBB 

ExtensionsBB 
{CC 
publicDD 

staticDD 
boolDD #
HasChangedOwnedEntitiesDD .
(DD. /
thisDD/ 3
EntityEntryDD4 ?
entryDD@ E
)DDE F
=>DDG I
entryEE 
.EE 

ReferencesEE 
.EE 
AnyEE 
(EE 
rEE 
=>EE !
rFF 
.FF 
TargetEntryFF 
!=FF 
nullFF !
&&FF" $
rGG 
.GG 
TargetEntryGG 
.GG 
MetadataGG "
.GG" #
IsOwnedGG# *
(GG* +
)GG+ ,
&&GG- /
(HH 
rHH 
.HH 
TargetEntryHH 
.HH 
StateHH  
==HH! #
EntityStateHH$ /
.HH/ 0
AddedHH0 5
||HH6 8
rHH9 :
.HH: ;
TargetEntryHH; F
.HHF G
StateHHG L
==HHM O
EntityStateHHP [
.HH[ \
ModifiedHH\ d
)HHd e
)HHe f
;HHf g
}II ›+
C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Repository\ApplicationDbContext.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <

Repository< F
;F G
public 
class  
ApplicationDbContext !
:" #
	DbContext$ -
,- .!
IApplicationDbContext/ D
{ 
private 
readonly "
IDomainEventDispatcher +
_dispatcher, 7
;7 8
private 
readonly 1
%AuditableEntitySaveChangesInterceptor :2
&_auditableEntitySaveChangesInterceptor; a
;a b
private 
readonly 
IEncryptionProvider (
	_provider) 2
;2 3
public 
 
ApplicationDbContext 
( 	
DbContextOptions 
<  
ApplicationDbContext 1
>1 2
options3 :
,: ;"
IDomainEventDispatcher "

dispatcher# -
,- .1
%AuditableEntitySaveChangesInterceptor 11
%auditableEntitySaveChangesInterceptor2 W
,W X
IConfiguration 
configuration (
) 	
: 	
base
 
( 
options 
) 
{ 
_dispatcher   
=   

dispatcher    
;    !2
&_auditableEntitySaveChangesInterceptor!! .
=!!/ 01
%auditableEntitySaveChangesInterceptor!!1 V
;!!V W
this"" 
."" 
	_provider"" 
="" 
new"" &
GenerateEncryptionProvider"" 7
(""7 8
configuration""8 E
.""E F
GetValue""F N
<""N O
string""O U
>""U V
(""V W
$str""W ^
)""^ _
)""_ `
;""` a
}## 
	protected%% 
override%% 
void%% 
OnModelCreating%% +
(%%+ ,
ModelBuilder%%, 8
modelBuilder%%9 E
)%%E F
{&& 
modelBuilder'' 
.'' +
ApplyConfigurationsFromAssembly'' 4
(''4 5
Assembly''5 =
.''= > 
GetExecutingAssembly''> R
(''R S
)''S T
)''T U
;''U V
modelBuilder)) 
.)) 
UseEncryption)) "
())" #
this))# '
.))' (
	_provider))( 1
)))1 2
;))2 3
base++ 
.++ 
OnModelCreating++ 
(++ 
modelBuilder++ )
)++) *
;++* +
},, 
	protected.. 
override.. 
void.. 
OnConfiguring.. )
(..) *#
DbContextOptionsBuilder..* A
optionsBuilder..B P
)..P Q
{// 
optionsBuilder00 
.00 &
EnableSensitiveDataLogging00 1
(001 2
)002 3
;003 4
optionsBuilder11 
.11 
AddInterceptors11 &
(11& '2
&_auditableEntitySaveChangesInterceptor11' M
)11M N
;11N O
}22 
public44 

override44 
async44 
Task44 
<44 
int44 "
>44" #
SaveChangesAsync44$ 4
(444 5
CancellationToken445 F
cancellationToken44G X
=44Y Z
new44[ ^
CancellationToken44_ p
(44p q
)44q r
)44r s
{55 
int66 
result66 
=66 
await66 
base66 
.66  
SaveChangesAsync66  0
(660 1
cancellationToken661 B
)66B C
.66C D
ConfigureAwait66D R
(66R S
false66S X
)66X Y
;66Y Z
var99 
entitiesWithEvents99 
=99  
ChangeTracker99! .
.:: 
Entries:: 
(:: 
):: 
.;; 
Select;; 
(;; 
e;; 
=>;; 
e;; 
.;; 
Entity;; !
as;;" $

EntityBase;;% /
<;;/ 0
string;;0 6
>;;6 7
);;7 8
.<< 
Where<< 
(<< 
e<< 
=><< 
e<< 
?<< 
.<< 
DomainEvents<< '
!=<<( *
null<<+ /
&&<<0 2
e<<3 4
.<<4 5
DomainEvents<<5 A
.<<A B
Any<<B E
(<<E F
)<<F G
)<<G H
.== 
ToArray== 
(== 
)== 
;== 
ifAA 

(AA 
entitiesWithEventsAA 
!=AA !
nullAA" &
&&AA' )
entitiesWithEventsAA* <
.AA< =
AnyAA= @
(AA@ A
)AAA B
)AAB C
{BB 	
}KK 	
returnOO 
resultOO 
;OO 
}PP 
publicRR 

DbSetRR 
<RR 
ReferralRR 
>RR 
	ReferralsRR $
=>RR% '
SetRR( +
<RR+ ,
ReferralRR, 4
>RR4 5
(RR5 6
)RR6 7
;RR7 8
publicSS 

DbSetSS 
<SS 
ReferralStatusSS 
>SS  
ReferralStatusesSS! 1
=>SS2 4
SetSS5 8
<SS8 9
ReferralStatusSS9 G
>SSG H
(SSH I
)SSI J
;SSJ K
}TT Ò#
äC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Repository\ApplicationDbContextInitialiser.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <

Repository< F
;F G
public 
class +
ApplicationDbContextInitialiser ,
{		 
private

 
readonly

 
ILogger

 
<

 +
ApplicationDbContextInitialiser

 <
>

< =
_logger

> E
;

E F
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
public 
+
ApplicationDbContextInitialiser *
(* +
ILogger+ 2
<2 3+
ApplicationDbContextInitialiser3 R
>R S
loggerT Z
,Z [ 
ApplicationDbContext\ p
contextq x
)x y
{ 
_logger 
= 
logger 
; 
_context 
= 
context 
; 
} 
public 

async 
Task 
InitialiseAsync %
(% &
IConfiguration& 4
configuration5 B
)B C
{ 
try 
{ 	
if 
( 
_context 
. 
Database !
.! "
IsSqlServer" -
(- .
). /
||0 2
_context3 ;
.; <
Database< D
.D E
IsNpgsqlE M
(M N
)N O
)O P
{ 
if 
( 
configuration !
.! "
GetValue" *
<* +
bool+ /
>/ 0
(0 1
$str1 F
)F G
)G H
{ 
_context 
. 
Database %
.% &
EnsureDeleted& 3
(3 4
)4 5
;5 6
_context 
. 
Database %
.% &
EnsureCreated& 3
(3 4
)4 5
;5 6
} 
else 
await 
_context "
." #
Database# +
.+ ,
MigrateAsync, 8
(8 9
)9 :
;: ;
}   
}!! 	
catch"" 
("" 
	Exception"" 
ex"" 
)"" 
{## 	
_logger$$ 
.$$ 
LogError$$ 
($$ 
ex$$ 
,$$  
$str$$! U
)$$U V
;$$V W
throw%% 
;%% 
}&& 	
}'' 
public)) 

async)) 
Task)) 
	SeedAsync)) 
())  
)))  !
{** 
try++ 
{,, 	
await-- 
TrySeedAsync-- 
(-- 
)--  
;--  !
}.. 	
catch// 
(// 
	Exception// 
ex// 
)// 
{00 	
_logger11 
.11 
LogError11 
(11 
ex11 
,11  
$str11! P
)11P Q
;11Q R
throw22 
;22 
}33 	
}44 
public66 

async66 
Task66 
TrySeedAsync66 "
(66" #
)66# $
{77 
if88 

(88 
_context88 
.88 
	Referrals88 
.88 
Any88 "
(88" #
)88# $
)88$ %
return99 
;99 
var;; 
referralSeedData;; 
=;; 
new;; "
ReferralSeedData;;# 3
(;;3 4
);;4 5
;;;5 6
IReadOnlyCollection== 
<== 
Referral== $
>==$ %
	referrals==& /
===0 1
referralSeedData==2 B
.==B C
SeedReferral==C O
(==O P
)==P Q
;==Q R
foreach?? 
(?? 
var?? 
referral?? 
in??  
	referrals??! *
)??* +
{@@ 	
_contextAA 
.AA 
	ReferralsAA 
.AA 
AddAA "
(AA" #
referralAA# +
)AA+ ,
;AA, -
}BB 	
awaitDD 
_contextDD 
.DD 
SaveChangesAsyncDD '
(DD' (
)DD( )
;DD) *
}FF 
}GG ÙÉ
{C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Repository\CachedRepository.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <

Repository< F
;F G
public 
class 
CachedRepository 
< 
T 
>  
:! "
IReadRepository# 2
<2 3
T3 4
>4 5
where6 ;
T< =
:> ?
class@ E
,E F
IAggregateRootG U
{		 
private

 
readonly

 
IMemoryCache

 !
_cache

" (
;

( )
private 
readonly 
ILogger 
< 
CachedRepository -
<- .
T. /
>/ 0
>0 1
_logger2 9
;9 :
private 
readonly 
EfRepository !
<! "
T" #
># $
_sourceRepository% 6
;6 7
private #
MemoryCacheEntryOptions #
_cacheOptions$ 1
;1 2
public 

CachedRepository 
( 
IMemoryCache (
cache) .
,. /
ILogger 
< 
CachedRepository  
<  !
T! "
>" #
># $
logger% +
,+ ,
EfRepository 
< 
T 
> 
sourceRepository (
)( )
{ 
_cache 
= 
cache 
; 
_logger 
= 
logger 
; 
_sourceRepository 
= 
sourceRepository ,
;, -
_cacheOptions 
= 
new #
MemoryCacheEntryOptions 3
(3 4
)4 5
. !
SetAbsoluteExpiration "
(" #
relative# +
:+ ,
TimeSpan- 5
.5 6
FromSeconds6 A
(A B
$numB D
)D E
)E F
;F G
} 
public 

Task 
< 
T 
> 
AddAsync 
( 
T 
entity $
)$ %
{ 
return 
_sourceRepository  
.  !
AddAsync! )
() *
entity* 0
)0 1
;1 2
} 
public   

Task   
<   
bool   
>   
AnyAsync   
(   
ISpecification   -
<  - .
T  . /
>  / 0
specification  1 >
,  > ?
CancellationToken  @ Q
cancellationToken  R c
=  d e
default  f m
)  m n
{!! 
throw"" 
new"" #
NotImplementedException"" )
("") *
)""* +
;""+ ,
}## 
public%% 

Task%% 
<%% 
bool%% 
>%% 
AnyAsync%% 
(%% 
CancellationToken%% 0
cancellationToken%%1 B
=%%C D
default%%E L
)%%L M
{&& 
throw'' 
new'' #
NotImplementedException'' )
('') *
)''* +
;''+ ,
}(( 
public** 

Task** 
<** 
int** 
>** 

CountAsync** 
(**  
ISpecification**  .
<**. /
T**/ 0
>**0 1
specification**2 ?
,**? @
CancellationToken**A R
cancellationToken**S d
=**e f
default**g n
)**n o
{++ 
return,, 
_sourceRepository,,  
.,,  !

CountAsync,,! +
(,,+ ,
specification,,, 9
,,,9 :
cancellationToken,,; L
),,L M
;,,M N
}-- 
public// 

Task// 
<// 
int// 
>// 

CountAsync// 
(//  
CancellationToken//  1
cancellationToken//2 C
=//D E
default//F M
)//M N
{00 
throw11 
new11 #
NotImplementedException11 )
(11) *
)11* +
;11+ ,
}22 
public44 

Task44 
DeleteAsync44 
(44 
T44 
entity44 $
)44$ %
{55 
return66 
_sourceRepository66  
.66  !
DeleteAsync66! ,
(66, -
entity66- 3
)663 4
;664 5
}77 
public99 

Task99 
DeleteRangeAsync99  
(99  !
IEnumerable99! ,
<99, -
T99- .
>99. /
entities990 8
)998 9
{:: 
return;; 
_sourceRepository;;  
.;;  !
DeleteRangeAsync;;! 1
(;;1 2
entities;;2 :
);;: ;
;;;; <
}<< 
public>> 

Task>> 
<>> 
T>> 
?>> 
>>> 
FirstOrDefaultAsync>> '
(>>' (
ISpecification>>( 6
<>>6 7
T>>7 8
>>>8 9
specification>>: G
,>>G H
CancellationToken>>I Z
cancellationToken>>[ l
=>>m n
default>>o v
)>>v w
{?? 
throw@@ 
new@@ #
NotImplementedException@@ )
(@@) *
)@@* +
;@@+ ,
}AA 
publicCC 

TaskCC 
<CC 
TResultCC 
?CC 
>CC 
FirstOrDefaultAsyncCC -
<CC- .
TResultCC. 5
>CC5 6
(CC6 7
ISpecificationCC7 E
<CCE F
TCCF G
,CCG H
TResultCCI P
>CCP Q
specificationCCR _
,CC_ `
CancellationTokenCCa r
cancellationToken	CCs Ñ
=
CCÖ Ü
default
CCá é
)
CCé è
{DD 
throwEE 
newEE #
NotImplementedExceptionEE )
(EE) *
)EE* +
;EE+ ,
}FF 
publicHH 

TaskHH 
<HH 
THH 
?HH 
>HH 
GetByIdAsyncHH  
<HH  !
TIdHH! $
>HH$ %
(HH% &
TIdHH& )
idHH* ,
,HH, -
CancellationTokenHH. ?
cancellationTokenHH@ Q
=HHR S
defaultHHT [
)HH[ \
whereHH] b
TIdHHc f
:HHg h
notnullHHi p
{II 
stringJJ 
?JJ 
keyJJ 
=JJ 
$"JJ 
{JJ 
typeofJJ 
(JJ  
TJJ  !
)JJ! "
.JJ" #
NameJJ# '
}JJ' (
$strJJ( )
{JJ) *
idJJ* ,
}JJ, -
"JJ- .
;JJ. /
_loggerKK 
.KK 
LogInformationKK 
(KK 
$strKK 4
+KK5 6
keyKK7 :
)KK: ;
;KK; <
returnLL 
_cacheLL 
.LL 
GetOrCreateLL !
(LL! "
keyLL" %
,LL% &
entryLL' ,
=>LL- /
{MM 	
ifNN 
(NN 
entryNN 
!=NN 
nullNN 
&&NN  
_cacheOptionsNN! .
!=NN/ 1
nullNN2 6
)NN6 7
{OO 
entryPP 
.PP 

SetOptionsPP  
(PP  !
_cacheOptionsPP! .
)PP. /
;PP/ 0
}QQ 
_loggerRR 
.RR 

LogWarningRR 
(RR 
$strRR :
+RR; <
keyRR= @
)RR@ A
;RRA B
returnSS 
_sourceRepositorySS $
.SS$ %
GetByIdAsyncSS% 1
(SS1 2
idSS2 4
,SS4 5
cancellationTokenSS6 G
)SSG H
;SSH I
}TT 	
)TT	 

;TT
 
}UU 
publicWW 

TaskWW 
<WW 
TWW 
?WW 
>WW 
GetBySpecAsyncWW "
(WW" #
ISpecificationWW# 1
<WW1 2
TWW2 3
>WW3 4
specificationWW5 B
,WWB C
CancellationTokenWWD U
cancellationTokenWWV g
=WWh i
defaultWWj q
)WWq r
{XX 
throwYY 
newYY #
NotImplementedExceptionYY )
(YY) *
)YY* +
;YY+ ,
}ZZ 
public\\ 

Task\\ 
<\\ 
TResult\\ 
?\\ 
>\\ 
GetBySpecAsync\\ (
<\\( )
TResult\\) 0
>\\0 1
(\\1 2
ISpecification\\2 @
<\\@ A
T\\A B
,\\B C
TResult\\D K
>\\K L
specification\\M Z
,\\Z [
CancellationToken\\\ m
cancellationToken\\n 
=
\\Ä Å
default
\\Ç â
)
\\â ä
{]] 
throw^^ 
new^^ #
NotImplementedException^^ )
(^^) *
)^^* +
;^^+ ,
}__ 
publicaa 

Taskaa 
<aa 
Listaa 
<aa 
Taa 
>aa 
>aa 
	ListAsyncaa "
(aa" #
CancellationTokenaa# 4
cancellationTokenaa5 F
=aaG H
defaultaaI P
)aaP Q
{bb 
stringcc 
keycc 
=cc 
$"cc 
{cc 
typeofcc 
(cc 
Tcc  
)cc  !
.cc! "
Namecc" &
}cc& '
$strcc' ,
"cc, -
;cc- .
_loggerdd 
.dd 
LogInformationdd 
(dd 
$"dd !
$strdd! 4
{dd4 5
keydd5 8
}dd8 9
"dd9 :
)dd: ;
;dd; <
returnee 
_cacheee 
.ee 
GetOrCreateee !
(ee! "
keyee" %
,ee% &
entryee' ,
=>ee- /
{ff 	
entrygg 
.gg 

SetOptionsgg 
(gg 
_cacheOptionsgg *
)gg* +
;gg+ ,
_loggerhh 
.hh 

LogWarninghh 
(hh 
$"hh !
$strhh! :
{hh: ;
keyhh; >
}hh> ?
"hh? @
)hh@ A
;hhA B
returnii 
_sourceRepositoryii $
.ii$ %
	ListAsyncii% .
(ii. /
cancellationTokenii/ @
)ii@ A
;iiA B
}jj 	
)jj	 

;jj
 
}kk 
publicmm 

Taskmm 
<mm 
Listmm 
<mm 
Tmm 
>mm 
>mm 
	ListAsyncmm "
(mm" #
ISpecificationmm# 1
<mm1 2
Tmm2 3
>mm3 4
specificationmm5 B
,mmB C
CancellationTokennn 
cancellationTokennn )
=nn* +
defaultnn, 3
)nn3 4
{oo 
ifpp 

(pp 
specificationpp 
.pp 
CacheEnabledpp &
)pp& '
{qq 	
stringrr 
keyrr 
=rr 
$"rr 
{rr 
specificationrr )
.rr) *
CacheKeyrr* 2
}rr2 3
$strrr3 =
"rr= >
;rr> ?
_loggerss 
.ss 
LogInformationss "
(ss" #
$"ss# %
$strss% 8
{ss8 9
keyss9 <
}ss< =
"ss= >
)ss> ?
;ss? @
returntt 
_cachett 
.tt 
GetOrCreatett %
(tt% &
keytt& )
,tt) *
entrytt+ 0
=>tt1 3
{uu 
entryvv 
.vv 

SetOptionsvv  
(vv  !
_cacheOptionsvv! .
)vv. /
;vv/ 0
_loggerww 
.ww 

LogWarningww "
(ww" #
$"ww# %
$strww% >
{ww> ?
keyww? B
}wwB C
"wwC D
)wwD E
;wwE F
returnxx 
_sourceRepositoryxx (
.xx( )
	ListAsyncxx) 2
(xx2 3
specificationxx3 @
,xx@ A
cancellationTokenxxB S
)xxS T
;xxT U
}yy 
)yy 
;yy 
}zz 	
return{{ 
_sourceRepository{{  
.{{  !
	ListAsync{{! *
({{* +
specification{{+ 8
,{{8 9
cancellationToken{{: K
){{K L
;{{L M
}|| 
public~~ 

Task~~ 
<~~ 
List~~ 
<~~ 
TResult~~ 
>~~ 
>~~ 
	ListAsync~~ (
<~~( )
TResult~~) 0
>~~0 1
(~~1 2
ISpecification~~2 @
<~~@ A
T~~A B
,~~B C
TResult~~D K
>~~K L
specification~~M Z
,~~Z [
CancellationToken 
cancellationToken )
=* +
default, 3
)3 4
{
ÄÄ 
if
ÅÅ 

(
ÅÅ 
specification
ÅÅ 
.
ÅÅ 
CacheEnabled
ÅÅ &
)
ÅÅ& '
{
ÇÇ 	
string
ÉÉ 
key
ÉÉ 
=
ÉÉ 
$"
ÉÉ 
{
ÉÉ 
specification
ÉÉ )
.
ÉÉ) *
CacheKey
ÉÉ* 2
}
ÉÉ2 3
$str
ÉÉ3 =
"
ÉÉ= >
;
ÉÉ> ?
_logger
ÑÑ 
.
ÑÑ 
LogInformation
ÑÑ "
(
ÑÑ" #
$"
ÑÑ# %
$str
ÑÑ% 8
{
ÑÑ8 9
key
ÑÑ9 <
}
ÑÑ< =
"
ÑÑ= >
)
ÑÑ> ?
;
ÑÑ? @
return
ÖÖ 
_cache
ÖÖ 
.
ÖÖ 
GetOrCreate
ÖÖ %
(
ÖÖ% &
key
ÖÖ& )
,
ÖÖ) *
entry
ÖÖ+ 0
=>
ÖÖ1 3
{
ÜÜ 
entry
áá 
.
áá 

SetOptions
áá  
(
áá  !
_cacheOptions
áá! .
)
áá. /
;
áá/ 0
_logger
àà 
.
àà 

LogWarning
àà "
(
àà" #
$"
àà# %
$str
àà% >
{
àà> ?
key
àà? B
}
ààB C
"
ààC D
)
ààD E
;
ààE F
return
ââ 
_sourceRepository
ââ (
.
ââ( )
	ListAsync
ââ) 2
(
ââ2 3
specification
ââ3 @
,
ââ@ A
cancellationToken
ââB S
)
ââS T
;
ââT U
}
ää 
)
ää 
;
ää 
}
ãã 	
return
åå 
_sourceRepository
åå  
.
åå  !
	ListAsync
åå! *
(
åå* +
specification
åå+ 8
,
åå8 9
cancellationToken
åå: K
)
ååK L
;
ååL M
}
çç 
public
èè 

Task
èè 
SaveChangesAsync
èè  
(
èè  !
)
èè! "
{
êê 
return
ëë 
_sourceRepository
ëë  
.
ëë  !
SaveChangesAsync
ëë! 1
(
ëë1 2
)
ëë2 3
;
ëë3 4
}
íí 
public
îî 

Task
îî 
<
îî 
T
îî 
?
îî 
>
îî "
SingleOrDefaultAsync
îî (
(
îî( )(
ISingleResultSpecification
îî) C
<
îîC D
T
îîD E
>
îîE F
specification
îîG T
,
îîT U
CancellationToken
îîV g
cancellationToken
îîh y
=
îîz {
defaultîî| É
)îîÉ Ñ
{
ïï 
throw
ññ 
new
ññ %
NotImplementedException
ññ )
(
ññ) *
)
ññ* +
;
ññ+ ,
}
óó 
public
ôô 

Task
ôô 
<
ôô 
TResult
ôô 
?
ôô 
>
ôô "
SingleOrDefaultAsync
ôô .
<
ôô. /
TResult
ôô/ 6
>
ôô6 7
(
ôô7 8(
ISingleResultSpecification
ôô8 R
<
ôôR S
T
ôôS T
,
ôôT U
TResult
ôôV ]
>
ôô] ^
specification
ôô_ l
,
ôôl m
CancellationToken
ôôn !
cancellationTokenôôÄ ë
=ôôí ì
defaultôôî õ
)ôôõ ú
{
öö 
throw
õõ 
new
õõ %
NotImplementedException
õõ )
(
õõ) *
)
õõ* +
;
õõ+ ,
}
úú 
public
ûû 

Task
ûû 
UpdateAsync
ûû 
(
ûû 
T
ûû 
entity
ûû $
)
ûû$ %
{
üü 
return
†† 
_sourceRepository
††  
.
††  !
UpdateAsync
††! ,
(
††, -
entity
††- 3
)
††3 4
;
††4 5
}
°° 
}¢¢ À
wC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Repository\EfRepository.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <

Repository< F
;F G
public 
class 
EfRepository 
< 
T 
> 
: 
RepositoryBase -
<- .
T. /
>/ 0
,0 1
IReadRepository2 A
<A B
TB C
>C D
,D E
IRepositoryF Q
<Q R
TR S
>S T
whereU Z
T[ \
:] ^
class_ d
,d e
IAggregateRootf t
{		 
public

 

EfRepository

 
(

  
ApplicationDbContext

 ,
	dbContext

- 6
)

6 7
:

8 9
base

: >
(

> ?
	dbContext

? H
)

H I
{ 
} 
} ∞
{C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Persistence\Repository\ReferralSeedData.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Persistence0 ;
.; <

Repository< F
;F G
public 
class 
ReferralSeedData 
{ 
const 	
string
 
JsonService 
= 
$str	 ´
;
´ ¨
public 

IReadOnlyCollection 
< 
Referral '
>' (
SeedReferral) 5
(5 6
)6 7
{		 
List

 
<

 
Referral

 
>

 
listReferrals

 $
=

% &
new

' *
(

* +
)

+ ,
{ 	
new 
Referral 
( 
$str 6
,6 7
$str 6
,6 7
$str 6
,6 7
$str D
,D E
$str	 ˇ
,
ˇ Ä
JsonService 
, 
$str "
," #
$str 
,  
$str 
, 
$str (
,( )
$str 
,  
$str 
,  
$str *
,* +
null 
, 
new 
List 
< 
ReferralStatus '
>' (
{) *
new+ .
ReferralStatus/ =
(= >
$str> d
,d e
$strf z
,z {
$str	| ¢
)
¢ £
}
§ •
) 
} 	
;	 

return 
listReferrals 
; 
}!! 
}&& µ
kC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Infrastructure\Service\DateTimeService.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Infrastructure! /
./ 0
Service0 7
;7 8
public 
class 
DateTimeService 
: 
	IDateTime (
{ 
public 

DateTime 
Now 
=> 
DateTime #
.# $
UtcNow$ *
;* +
} 