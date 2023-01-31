€#
_C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\CommandMessageConsumer.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Api		! $
;		$ %
[ #
ExcludeFromCodeCoverage 
] 
public 
class "
CommandMessageConsumer #
:$ %
	IConsumer& /
</ 0
CommandMessage0 >
>> ?
{ 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
CommandMessage- ;
>; <
context= D
)D E
{ 
var 
message 
= 
context 
. 
Message %
;% &
await 
Console 
. 
Out 
. 
WriteLineAsync (
(( )
$") +
$str+ C
{C D
messageD K
.K L
MessageStringL Y
}Y Z
"Z [
)[ \
;\ ]
using 
( 
var 
scope 
= 
Program "
." #
ServiceProvider# 2
.2 3
CreateScope3 >
(> ?
)? @
)@ A
{ 	
ILogger 
< "
CommandMessageConsumer *
>* +
?+ ,
logger- 3
=4 5
null6 :
;: ;
try 
{ 
logger 
= 
scope 
. 
ServiceProvider .
.. /

GetService/ 9
<9 :
ILogger: A
<A B"
CommandMessageConsumerB X
>X Y
>Y Z
(Z [
)[ \
;\ ]
if 
( 
context 
!= 
null #
&&$ &
context' .
.. /
Message/ 6
!=7 9
null: >
&&? A
!B C
stringC I
.I J
IsNullOrEmptyJ W
(W X
contextX _
._ `
Message` g
.g h
MessageStringh u
)u v
)v w
{ 
ReferralDto 
?  
dto! $
=% &
JsonSerializer' 5
.5 6
Deserialize6 A
<A B
ReferralDtoB M
>M N
(N O
contextO V
.V W
MessageW ^
.^ _
MessageString_ l
,l m
optionsn u
:u v
neww z"
JsonSerializerOptions	{ ê
{
ë í)
PropertyNameCaseInsensitive
ì Æ
=
Ø ∞
true
± µ
}
∂ ∑
)
∑ ∏
;
∏ π
if 
( 
dto 
!= 
null #
)# $
{ !
CreateReferralCommand -
command. 5
=6 7
new8 ;
(; <
dto< ?
)? @
;@ A
var   
mediator   $
=  % &
scope  ' ,
.  , -
ServiceProvider  - <
.  < =

GetService  = G
<  G H
ISender  H O
>  O P
(  P Q
)  Q R
;  R S
if!! 
(!! 
mediator!! $
!=!!% '
null!!( ,
)!!, -
{"" 
var## 
result##  &
=##' (
await##) .
mediator##/ 7
.##7 8
Send##8 <
(##< =
command##= D
,##D E
new##F I
CancellationToken##J [
(##[ \
)##\ ]
)##] ^
;##^ _
}$$ 
}%% 
}&& 
}'' 
catch(( 
((( 
	Exception(( 
ex(( 
)((  
{)) 
System** 
.** 
Diagnostics** "
.**" #
Debug**# (
.**( )
	WriteLine**) 2
(**2 3
ex**3 5
.**5 6
Message**6 =
)**= >
;**> ?
if++ 
(++ 
logger++ 
!=++ 
null++ "
)++" #
{,, 
logger-- 
.-- 
LogError-- #
(--# $
ex--$ &
,--& '
$str--( O
)--O P
;--P Q
}.. 
throw00 
;00 
}11 
}22 	
}33 
}44 ˙-
vC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\CreateReferral\CreateReferralCommand.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Api		! $
.		$ %
Commands		% -
.		- .
CreateReferral		. <
;		< =
public 
class !
CreateReferralCommand "
:# $
IRequest% -
<- .
string. 4
>4 5
,5 6"
ICreateReferralCommand7 M
{ 
public 
!
CreateReferralCommand  
(  !
ReferralDto! ,
referralDto- 8
)8 9
{ 
ReferralDto 
= 
referralDto !
;! "
} 
public 

ReferralDto 
ReferralDto "
{# $
get% (
;( )
}* +
} 
public 
class (
CreateReferralCommandHandler )
:* +
IRequestHandler, ;
<; <!
CreateReferralCommand< Q
,Q R
stringS Y
>Y Z
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
private 
readonly 
IMapper 
_mapper $
;$ %
private 
readonly 
ILogger 
< (
CreateReferralCommandHandler 9
>9 :
_logger; B
;B C
public 
(
CreateReferralCommandHandler '
(' ( 
ApplicationDbContext( <
context= D
,D E
IMapperF M
mapperN T
,T U
ILoggerV ]
<] ^(
CreateReferralCommandHandler^ z
>z {
logger	| Ç
)
Ç É
{ 
_logger 
= 
logger 
; 
_context 
= 
context 
; 
_mapper 
= 
mapper 
; 
} 
public   

async   
Task   
<   
string   
>   
Handle   $
(  $ %!
CreateReferralCommand  % :
request  ; B
,  B C
CancellationToken  D U
cancellationToken  V g
)  g h
{!! 
try"" 
{## 	
var$$ 
entity$$ 
=$$ 
_mapper$$  
.$$  !
Map$$! $
<$$$ %
Referral$$% -
>$$- .
($$. /
request$$/ 6
.$$6 7
ReferralDto$$7 B
)$$B C
;$$C D!
ArgumentNullException%% !
.%%! "
ThrowIfNull%%" -
(%%- .
entity%%. 4
)%%4 5
;%%5 6
if'' 
('' 
entity'' 
.'' 
Status'' 
!=''  
null''! %
)''% &
{(( 
for)) 
()) 
int)) 
i)) 
=)) 
entity)) #
.))# $
Status))$ *
.))* +
Count))+ 0
-))1 2
$num))3 4
;))4 5
i))6 7
>=))8 :
$num)); <
;))< =
i))> ?
--))? A
)))A B
{** 
var++ 
referralStatus++ &
=++' (
_context++) 1
.++1 2
ReferralStatuses++2 B
.++B C
FirstOrDefault++C Q
(++Q R
x++R S
=>++T V
x++W X
.++X Y
Id++Y [
==++\ ^
entity++_ e
.++e f
Status++f l
.++l m
	ElementAt++m v
(++v w
i++w x
)++x y
.++y z
Id++z |
)++| }
;++} ~
if,, 
(,, 
referralStatus,, &
!=,,' )
null,,* .
),,. /
{-- 
entity.. 
... 
Status.. %
...% &
Remove..& ,
(.., -
entity..- 3
...3 4
Status..4 :
...: ;
	ElementAt..; D
(..D E
i..E F
)..F G
)..G H
;..H I
entity// 
.// 
Status// %
.//% &
Add//& )
(//) *
referralStatus//* 8
)//8 9
;//9 :
}00 
}11 
}22 
entity44 
.44 
RegisterDomainEvent44 &
(44& '
new44' * 
ReferralCreatedEvent44+ ?
(44? @
entity44@ F
)44F G
)44G H
;44H I
_context55 
.55 
	Referrals55 
.55 
Add55 "
(55" #
entity55# )
)55) *
;55* +
await66 
_context66 
.66 
SaveChangesAsync66 +
(66+ ,
cancellationToken66, =
)66= >
;66> ?
}77 	
catch88 
(88 
	Exception88 
ex88 
)88 
{99 	
_logger:: 
.:: 
LogError:: 
(:: 
ex:: 
,::  
$str::! Z
,::Z [
ex::\ ^
.::^ _
Message::_ f
)::f g
;::g h
throw;; 
;;; 
}<< 	
if>> 

(>> 
request>> 
is>> 
not>> 
null>> 
&&>>  "
request>># *
.>>* +
ReferralDto>>+ 6
is>>7 9
not>>: =
null>>> B
)>>B C
return?? 
request?? 
.?? 
ReferralDto?? &
.??& '
Id??' )
;??) *
else@@ 
returnAA 
stringAA 
.AA 
EmptyAA 
;AA  
}BB 
}CC è
C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\CreateReferral\CreateReferralCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Commands% -
.- .
CreateReferral. <
;< =
public 
class *
CreateReferralCommandValidator +
:, -
AbstractValidator. ?
<? @!
CreateReferralCommand@ U
>U V
{ 
public 
*
CreateReferralCommandValidator )
() *
)* +
{ 
RuleFor		 
(		 
v		 
=>		 
v		 
.		 
ReferralDto		 "
)		" #
.

 
NotNull

 
(

 
)

 
;

 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
Id# %
)% &
. 
MinimumLength 
( 
$num 
) 
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
	ServiceId# ,
), -
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
ServiceName# .
). /
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
Referrer# +
)+ ,
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor   
(   
v   
=>   
v   
.   
ReferralDto   "
.  " #
FullName  # +
)  + ,
.!! 
MinimumLength!! 
(!! 
$num!! 
)!! 
."" 
NotNull"" 
("" 
)"" 
.## 
NotEmpty## 
(## 
)## 
;## 
}$$ 
}%% )
|C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\SetReferralStatus\SetReferralStatusCommand.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Commands% -
.- .
SetReferralStatus. ?
;? @
public		 
class		 $
SetReferralStatusCommand		 %
:		% &
IRequest		' /
<		/ 0
string		0 6
>		6 7
,		7 8%
ISetReferralStatusCommand		9 R
{

 
public 
$
SetReferralStatusCommand #
(# $
string$ *

referralId+ 5
,5 6
string7 =
status> D
)D E
{ 
Status 
= 
status 
; 

ReferralId 
= 

referralId 
;  
} 
public 

string 

ReferralId 
{ 
get "
;" #
}$ %
public 

string 
Status 
{ 
get 
; 
}  !
} 
public 
class .
"CreateReferralStatusCommandHandler /
:0 1
IRequestHandler2 A
<A B$
SetReferralStatusCommandB Z
,Z [
string\ b
>b c
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
private 
readonly 
ILogger 
< .
"CreateReferralStatusCommandHandler ?
>? @
_loggerA H
;H I
public 
.
"CreateReferralStatusCommandHandler -
(- . 
ApplicationDbContext. B
contextC J
,J K
ILoggerL S
<S T.
"CreateReferralStatusCommandHandlerT v
>v w
loggerx ~
)~ 
{ 
_logger 
= 
logger 
; 
_context 
= 
context 
; 
} 
public 

async 
Task 
< 
string 
> 
Handle $
($ %$
SetReferralStatusCommand% =
request> E
,E F
CancellationTokenG X
cancellationTokenY j
)j k
{   
try!! 
{"" 	
var## 
currentStatus## 
=## 
_context##  (
.##( )
ReferralStatuses##) 9
.##9 :
Where##: ?
(##? @
x##@ A
=>##B D
x##E F
.##F G

ReferralId##G Q
==##R T
request##U \
.##\ ]

ReferralId##] g
)##g h
.##h i
OrderBy##i p
(##p q
x##q r
=>##s u
x##v w
.##w x
LastModified	##x Ñ
)
##Ñ Ö
.
##Ö Ü
LastOrDefault
##Ü ì
(
##ì î
)
##î ï
;
##ï ñ
if$$ 
($$ 
currentStatus$$ 
!=$$  
null$$! %
&&$$& (
currentStatus$$) 6
.$$6 7
Status$$7 =
==$$> @
request$$A H
.$$H I
Status$$I O
)$$O P
{%% 
return&& 
currentStatus&& $
.&&$ %
Status&&% +
;&&+ ,
}'' 
var(( 
entity(( 
=(( 
new(( 
ReferralStatus(( +
(((+ ,
Guid((, 0
.((0 1
NewGuid((1 8
(((8 9
)((9 :
.((: ;
ToString((; C
(((C D
)((D E
,((E F
request((G N
.((N O
Status((O U
,((U V
request((W ^
.((^ _

ReferralId((_ i
)((i j
;((j k
entity)) 
.)) 
RegisterDomainEvent)) &
())& '
new))' *&
ReferralStatusCreatedEvent))+ E
())E F
entity))F L
)))L M
)))M N
;))N O
_context** 
.** 
ReferralStatuses** %
.**% &
Add**& )
(**) *
entity*** 0
)**0 1
;**1 2
await++ 
_context++ 
.++ 
SaveChangesAsync++ +
(+++ ,
cancellationToken++, =
)++= >
;++> ?
},, 	
catch-- 
(-- 
	Exception-- 
ex-- 
)-- 
{.. 	
_logger// 
.// 
LogError// 
(// 
ex// 
,//  
$str//! Z
,//Z [
ex//\ ^
.//^ _
Message//_ f
)//f g
;//g h
throw00 
;00 
}11 	
if33 

(33 
request33 
is33 
not33 
null33 
&&33  "
request33# *
.33* +
Status33+ 1
is332 4
not335 8
null339 =
)33= >
return44 
request44 
.44 
Status44 !
;44! "
else55 
return66 
string66 
.66 
Empty66 
;66  
}77 
}88 ⁄
ÖC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\SetReferralStatus\SetReferralStatusCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Commands% -
.- .
SetReferralStatus. ?
;? @
public 
class -
!SetReferralStatusCommandValidator .
:/ 0
AbstractValidator1 B
<B C$
SetReferralStatusCommandC [
>[ \
{ 
public		 
-
!SetReferralStatusCommandValidator		 ,
(		, -
)		- .
{

 
RuleFor 
( 
v 
=> 
v 
. 

ReferralId !
)! "
. 
MinimumLength 
( 
$num 
) 
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Status 
) 
. 
MinimumLength 
( 
$num 
) 
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
} 
} ó8
vC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\UpdateReferral\UpdateReferralCommand.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Api		! $
.		$ %
Commands		% -
.		- .
UpdateReferral		. <
;		< =
public 
class !
UpdateReferralCommand "
:# $
IRequest% -
<- .
string. 4
>4 5
,5 6"
IUpdateReferralCommand7 M
{ 
public 
!
UpdateReferralCommand  
(  !
string! '
id( *
,* +
ReferralDto, 7
referralDto8 C
)C D
{ 
Id 

= 
id 
; 
ReferralDto 
= 
referralDto !
;! "
} 
public 

string 
Id 
{ 
get 
; 
} 
public 

ReferralDto 
ReferralDto "
{# $
get% (
;( )
}* +
} 
public 
class (
UpdateReferralCommandHandler )
:* +
IRequestHandler, ;
<; <!
UpdateReferralCommand< Q
,Q R
stringS Y
>Y Z
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
private 
readonly 
IMapper 
_mapper $
;$ %
private 
readonly 
ILogger 
< (
UpdateReferralCommandHandler 9
>9 :
_logger; B
;B C
public 
(
UpdateReferralCommandHandler '
(' ( 
ApplicationDbContext( <
context= D
,D E
IMapperF M
mapperN T
,T U
ILoggerV ]
<] ^(
UpdateReferralCommandHandler^ z
>z {
logger	| Ç
)
Ç É
{ 
_logger 
= 
logger 
; 
_context 
= 
context 
; 
_mapper   
=   
mapper   
;   
}!! 
public"" 

async"" 
Task"" 
<"" 
string"" 
>"" 
Handle"" $
(""$ %!
UpdateReferralCommand""% :
request""; B
,""B C
CancellationToken""D U
cancellationToken""V g
)""g h
{## 
var$$ 
entity$$ 
=$$ 
_context$$ 
.$$ 
	Referrals$$ '
.$$' (
FirstOrDefault$$( 6
($$6 7
x$$7 8
=>$$9 ;
x$$< =
.$$= >
Id$$> @
==$$A C
request$$D K
.$$K L
Id$$L N
)$$N O
;$$O P
if%% 

(%% 
entity%% 
==%% 
null%% 
)%% 
{&& 	
throw'' 
new'' 
NotFoundException'' '
(''' (
nameof''( .
(''. /
Referral''/ 7
)''7 8
,''8 9
request'': A
.''A B
Id''B D
)''D E
;''E F
}(( 	
try** 
{++ 	
entity,, 
.,, 
OrganisationId,, !
=,," #
request,,$ +
.,,+ ,
ReferralDto,,, 7
.,,7 8
OrganisationId,,8 F
;,,F G
entity-- 
.-- 
	ServiceId-- 
=-- 
request-- &
.--& '
ReferralDto--' 2
.--2 3
	ServiceId--3 <
;--< =
entity.. 
... 
ServiceName.. 
=..  
request..! (
...( )
ReferralDto..) 4
...4 5
ServiceName..5 @
;..@ A
entity// 
.// 
ServiceDescription// %
=//& '
request//( /
./// 0
ReferralDto//0 ;
.//; <
ServiceDescription//< N
;//N O
entity00 
.00 
ServiceAsJson00  
=00! "
request00# *
.00* +
ReferralDto00+ 6
.006 7
ServiceAsJson007 D
;00D E
entity11 
.11 
Referrer11 
=11 
request11 %
.11% &
ReferralDto11& 1
.111 2
Referrer112 :
;11: ;
entity22 
.22 
FullName22 
=22 
request22 %
.22% &
ReferralDto22& 1
.221 2
FullName222 :
;22: ;
entity33 
.33 
HasSpecialNeeds33 "
=33# $
request33% ,
.33, -
ReferralDto33- 8
.338 9
HasSpecialNeeds339 H
;33H I
entity44 
.44 
Email44 
=44 
request44 "
.44" #
ReferralDto44# .
.44. /
Email44/ 4
??445 7
string448 >
.44> ?
Empty44? D
;44D E
entity55 
.55 
Phone55 
=55 
request55 "
.55" #
ReferralDto55# .
.55. /
Phone55/ 4
??555 7
string558 >
.55> ?
Empty55? D
;55D E
entity66 
.66 
Text66 
=66 
request66 !
.66! "
ReferralDto66" -
.66- .
Text66. 2
??663 5
string666 <
.66< =
Empty66= B
;66B C
entity77 
.77 
ReasonForSupport77 #
=77$ %
request77& -
.77- .
ReferralDto77. 9
.779 :
ReasonForSupport77: J
;77J K
entity88 
.88 
ReasonForRejection88 %
=88& '
request88( /
.88/ 0
ReferralDto880 ;
.88; <
ReasonForRejection88< N
;88N O
await:: 
_context:: 
.:: 
SaveChangesAsync:: +
(::+ ,
cancellationToken::, =
)::= >
;::> ?
};; 	
catch<< 
(<< 
	Exception<< 
ex<< 
)<< 
{== 	
_logger>> 
.>> 
LogError>> 
(>> 
ex>> 
,>>  
$str>>! Z
,>>Z [
ex>>\ ^
.>>^ _
Message>>_ f
)>>f g
;>>g h
throw?? 
;?? 
}@@ 	
ifBB 

(BB 
requestBB 
isBB 
notBB 
nullBB 
&&BB  "
requestBB# *
.BB* +
ReferralDtoBB+ 6
isBB7 9
notBB: =
nullBB> B
)BBB C
returnCC 
requestCC 
.CC 
ReferralDtoCC &
.CC& '
IdCC' )
;CC) *
elseDD 
returnEE 
stringEE 
.EE 
EmptyEE 
;EE  
}FF 
}GG è
C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Commands\UpdateReferral\UpdateReferralCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Commands% -
.- .
UpdateReferral. <
;< =
public 
class *
UpdateReferralCommandValidator +
:, -
AbstractValidator. ?
<? @!
UpdateReferralCommand@ U
>U V
{ 
public 
*
UpdateReferralCommandValidator )
() *
)* +
{ 
RuleFor		 
(		 
v		 
=>		 
v		 
.		 
ReferralDto		 "
)		" #
.

 
NotNull

 
(

 
)

 
;

 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
Id# %
)% &
. 
MinimumLength 
( 
$num 
) 
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
	ServiceId# ,
), -
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
ServiceName# .
). /
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ReferralDto "
." #
Referrer# +
)+ ,
. 
MaximumLength 
( 
$num 
) 
. 
NotNull 
( 
) 
. 
NotEmpty 
( 
) 
; 
RuleFor   
(   
v   
=>   
v   
.   
ReferralDto   "
.  " #
FullName  # +
)  + ,
.!! 
MinimumLength!! 
(!! 
$num!! 
)!! 
."" 
NotNull"" 
("" 
)"" 
.## 
NotEmpty## 
(## 
)## 
;## 
}$$ 
}%% ƒ
ZC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\ConfigureServices.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
public

 
static

 
class

 
ConfigureServices

 %
{ 
public 

static 
IServiceCollection $"
AddApplicationServices% ;
(; <
this< @
IServiceCollectionA S
servicesT \
)\ ]
{ 
services 
. 
AddTransient 
< 
	IDateTime '
,' (
DateTimeService) 8
>8 9
(9 :
): ;
;; <
services 
. 
AddTransient 
< 
ICurrentUserService 1
,1 2
CurrentUserService3 E
>E F
(F G
)G H
;H I
services 
. 
AddTransient 
<  
IHttpContextAccessor 2
,2 3
HttpContextAccessor4 G
>G H
(H I
)I J
;J K
var 
config 
= 
new 
MapperConfiguration ,
(, -
cfg- 0
=>1 3
{ 	
cfg 
. 

AddProfile 
( 
new 
AutoMappingProfiles 2
(2 3
)3 4
)4 5
;5 6
} 	
)	 

;
 
var 
mapper 
= 
config 
. 
CreateMapper (
(( )
)) *
;* +
services 
. 
AddSingleton 
( 
mapper $
)$ %
;% &
services 
. 
AddAutoMapper 
( 
Assembly '
.' ( 
GetExecutingAssembly( <
(< =
)= >
)> ?
;? @
services 
. 

AddMediatR 
( 
Assembly $
.$ % 
GetExecutingAssembly% 9
(9 :
): ;
); <
;< =
return 
services 
; 
}   
}!! ¬	
[C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\CurrentUserService.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
[ #
ExcludeFromCodeCoverage 
] 
public 
class 
CurrentUserService 
:  !
ICurrentUserService" 5
{		 
private

 
readonly

  
IHttpContextAccessor

 ) 
_httpContextAccessor

* >
;

> ?
public 

CurrentUserService 
(  
IHttpContextAccessor 2
httpContextAccessor3 F
)F G
{  
_httpContextAccessor 
= 
httpContextAccessor 2
;2 3
} 
public 

string 
? 
UserId 
=>  
_httpContextAccessor 1
.1 2
HttpContext2 =
?= >
.> ?
User? C
?C D
.D E
FindFirstValueE S
(S T

ClaimTypesT ^
.^ _
NameIdentifier_ m
)m n
;n o
} â 
_C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\DatabaseContextFactory.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
public		 
class		 "
DatabaseContextFactory		 #
:		$ %'
IDesignTimeDbContextFactory		& A
<		A B 
ApplicationDbContext		B V
>		V W
{

 
public 
 
ApplicationDbContext 
CreateDbContext  /
(/ 0
string0 6
[6 7
]7 8
args9 =
)= >
{ 
IConfigurationRoot 
configuration (
=) *
new+ . 
ConfigurationBuilder/ C
(C D
)D E
. 
SetBasePath 
( 
	Directory "
." #
GetCurrentDirectory# 6
(6 7
)7 8
)8 9
. 
AddJsonFile 
( 
$str +
)+ ,
. 
Build 
( 
) 
; 
var 
builder 
= 
new #
DbContextOptionsBuilder 1
<1 2 
ApplicationDbContext2 F
>F G
(G H
)H I
;I J
string 
	useDbType 
= 
configuration (
.( )
GetValue) 1
<1 2
string2 8
>8 9
(9 :
$str: E
)E F
;F G
switch 
( 
	useDbType 
) 
{ 	
default 
: 
builder 
. 
UseInMemoryDatabase +
(+ ,
$str, 8
)8 9
;9 :
break 
; 
case 
$str '
:' (
{ 
var 
connectionString (
=) *
configuration+ 8
.8 9
GetConnectionString9 L
(L M
$strM a
)a b
;b c
if 
( 
connectionString (
!=) +
null, 0
)0 1
builder   
.    
UseSqlServer    ,
(  , -
connectionString  - =
,  = >
b  ? @
=>  A C
b  D E
.  E F
MigrationsAssembly  F X
(  X Y
$str  Y u
)  u v
)  v w
;  w x
}"" 
break## 
;## 
case%% 
$str%% &
:%%& '
{&& 
var'' 
connectionString'' (
='') *
configuration''+ 8
.''8 9
GetConnectionString''9 L
(''L M
$str''M a
)''a b
;''b c
if(( 
((( 
connectionString(( (
!=(() +
null((, 0
)((0 1
builder)) 
.))  
	UseNpgsql))  )
())) *
connectionString))* :
,)): ;
b))< =
=>))> @
b))A B
.))B C
MigrationsAssembly))C U
())U V
$str))V r
)))r s
)))s t
;))t u
}++ 
break,, 
;,, 
}-- 	1
%AuditableEntitySaveChangesInterceptor// -1
%auditableEntitySaveChangesInterceptor//. S
=//T U
new//V Y
(//Y Z
new//Z ]
CurrentUserService//^ p
(//p q
new//q t 
HttpContextAccessor	//u à
(
//à â
)
//â ä
)
//ä ã
,
//ã å
new
//ç ê
DateTimeService
//ë †
(
//† °
)
//° ¢
)
//¢ £
;
//£ §
return22 
new22  
ApplicationDbContext22 '
(22' (
builder22( /
.22/ 0
Options220 7
,227 8
null229 =
,22= >1
%auditableEntitySaveChangesInterceptor22? d
,22d e
configuration22f s
)22s t
;22t u
}44 
}55 Ò
jC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Endpoints\MinimalGeneralEndPoints.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
	Endpoints% .
;. /
public 
class #
MinimalGeneralEndPoints $
{ 
public 

void +
RegisterMinimalGeneralEndPoints /
(/ 0
WebApplication0 >
app? B
)B C
{ 
app		 
.		 
MapGet		 
(		 
$str		 
,		 
(		  
ILogger		  '
<		' (#
MinimalGeneralEndPoints		( ?
>		? @
logger		A G
)		G H
=>		I K
{

 	
try 
{ 
var 
assembly 
= 
typeof %
(% &
	WebMarker& /
)/ 0
.0 1
Assembly1 9
;9 :
var 
creationDate  
=! "
File# '
.' (
GetCreationTime( 7
(7 8
assembly8 @
.@ A
LocationA I
)I J
;J K
var 
version 
= 
FileVersionInfo -
.- .
GetVersionInfo. <
(< =
assembly= E
.E F
LocationF N
)N O
.O P
ProductVersionP ^
;^ _
return 
Results 
. 
Ok !
(! "
$"" $
$str$ -
{- .
version. 5
}5 6
$str6 F
{F G
creationDateG S
}S T
"T U
)U V
;V W
} 
catch 
( 
	Exception 
ex 
)  
{ 
logger 
. 
LogError 
(  
ex  "
," #
$str$ ^
,^ _
ex` b
.b c
Messagec j
)j k
;k l
Debug 
. 
	WriteLine 
(  
ex  "
." #
Message# *
)* +
;+ ,
throw 
; 
} 
} 	
)	 

;
 
} 
} üd
kC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Endpoints\MinimalReferralEndPoints.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
	Endpoints% .
;. /
public 
class $
MinimalReferralEndPoints %
{ 
public 

void %
RegisterReferralEndPoints )
() *
WebApplication* 8
app9 <
)< =
{ 
app 
. 
MapPost 
( 
$str #
,# $
[% &
	Authorize& /
(/ 0
Policy0 6
=7 8
$str9 C
)C D
]D E
asyncF K
(L M
[M N
FromBodyN V
]V W
ReferralDtoX c
requestd k
,k l
CancellationTokenm ~
cancellationToken	 ê
,
ê ë
ISender
í ô
	_mediator
ö £
)
£ §
=>
• ß
{ 	
try 
{ !
CreateReferralCommand %
command& -
=. /
new0 3
(3 4
request4 ;
); <
;< =
var 
result 
= 
await "
	_mediator# ,
., -
Send- 1
(1 2
command2 9
,9 :
cancellationToken; L
)L M
;M N
return 
result 
; 
} 
catch 
( 
	Exception 
ex 
)  
{ 
System 
. 
Diagnostics "
." #
Debug# (
.( )
	WriteLine) 2
(2 3
ex3 5
.5 6
Message6 =
)= >
;> ?
throw 
; 
} 
} 	
)	 

.
 
WithMetadata 
( 
new %
SwaggerOperationAttribute 5
(5 6
$str6 A
,A B
$strC T
)T U
{V W
TagsX \
=] ^
new_ b
[b c
]c d
{e f
$strg r
}s t
}u v
)v w
;w x
app   
.   
MapPut   
(   
$str   '
,  ' (
[  ) *
	Authorize  * 3
(  3 4
Policy  4 :
=  ; <
$str  = G
)  G H
]  H I
async  J O
(  P Q
string  Q W
id  X Z
,  Z [
[  \ ]
FromBody  ] e
]  e f
ReferralDto  g r
request  s z
,  z {
CancellationToken	  | ç
cancellationToken
  é ü
,
  ü †
ISender
  ° ®
	_mediator
  © ≤
,
  ≤ ≥
ILogger
  ¥ ª
<
  ª º&
MinimalReferralEndPoints
  º ‘
>
  ‘ ’
logger
  ÷ ‹
)
  ‹ ›
=>
  ﬁ ‡
{!! 	
try"" 
{## !
UpdateReferralCommand$$ %
command$$& -
=$$. /
new$$0 3
($$3 4
id$$4 6
,$$6 7
request$$8 ?
)$$? @
;$$@ A
var%% 
result%% 
=%% 
await%% "
	_mediator%%# ,
.%%, -
Send%%- 1
(%%1 2
command%%2 9
,%%9 :
cancellationToken%%; L
)%%L M
;%%M N
return&& 
result&& 
;&& 
}'' 
catch(( 
((( 
	Exception(( 
ex(( 
)((  
{)) 
logger** 
.** 
LogError** 
(**  
ex**  "
,**" #
$str**$ c
,**c d
ex**e g
.**g h
Message**h o
)**o p
;**p q
System++ 
.++ 
Diagnostics++ "
.++" #
Debug++# (
.++( )
	WriteLine++) 2
(++2 3
ex++3 5
.++5 6
Message++6 =
)++= >
;++> ?
throw,, 
;,, 
}-- 
}.. 	
)..	 

...
 
WithMetadata.. 
(.. 
new.. %
SwaggerOperationAttribute.. 5
(..5 6
$str..6 G
,..G H
$str..I `
)..` a
{..b c
Tags..d h
=..i j
new..k n
[..n o
]..o p
{..q r
$str..s ~
}	.. Ä
}
..Å Ç
)
..Ç É
;
..É Ñ
app11 
.11 
MapGet11 
(11 
$str11 -
,11- .
[11/ 0
	Authorize110 9
(119 :
Policy11: @
=11A B
$str11C M
)11M N
]11N O
async11P U
(11V W
string11W ]
referrer11^ f
,11f g
int11h k
?11k l

pageNumber11m w
,11w x
int11y |
?11| }
pageSize	11~ Ü
,
11Ü á
CancellationToken
11à ô
cancellationToken
11ö ´
,
11´ ¨
ISender
11≠ ¥
	_mediator
11µ æ
)
11æ ø
=>
11¿ ¬
{22 	
try33 
{44 )
GetReferralsByReferrerCommand55 -
request55. 5
=556 7
new558 ;
(55; <
referrer55< D
,55D E

pageNumber55F P
,55P Q
pageSize55R Z
)55Z [
;55[ \
var66 
result66 
=66 
await66 "
	_mediator66# ,
.66, -
Send66- 1
(661 2
request662 9
,669 :
cancellationToken66; L
)66L M
;66M N
return77 
result77 
;77 
}88 
catch99 
(99 
	Exception99 
ex99 
)99  
{:: 
System;; 
.;; 
Diagnostics;; "
.;;" #
Debug;;# (
.;;( )
	WriteLine;;) 2
(;;2 3
ex;;3 5
.;;5 6
Message;;6 =
);;= >
;;;> ?
throw<< 
;<< 
}== 
}>> 	
)>>	 

.>>
 
WithMetadata>> 
(>> 
new>> %
SwaggerOperationAttribute>> 5
(>>5 6
$str>>6 E
,>>E F
$str>>G b
)>>b c
{>>d e
Tags>>f j
=>>k l
new>>m p
[>>p q
]>>q r
{>>s t
$str	>>u Ä
}
>>Å Ç
}
>>É Ñ
)
>>Ñ Ö
;
>>Ö Ü
app@@ 
.@@ 
MapGet@@ 
(@@ 
$str@@ ?
,@@? @
[@@A B
	Authorize@@B K
(@@K L
Policy@@L R
=@@S T
$str@@U _
)@@_ `
]@@` a
async@@b g
(@@h i
string@@i o
organisationId@@p ~
,@@~ 
int
@@Ä É
?
@@É Ñ

pageNumber
@@Ö è
,
@@è ê
int
@@ë î
?
@@î ï
pageSize
@@ñ û
,
@@û ü
CancellationToken
@@† ±
cancellationToken
@@≤ √
,
@@√ ƒ
ISender
@@≈ Ã
	_mediator
@@Õ ÷
)
@@÷ ◊
=>
@@ÿ ⁄
{AA 	
tryBB 
{CC /
#GetReferralsByOrganisationIdCommandDD 3
requestDD4 ;
=DD< =
newDD> A
(DDA B
organisationIdDDB P
,DDP Q

pageNumberDDR \
,DD\ ]
pageSizeDD^ f
)DDf g
;DDg h
varEE 
resultEE 
=EE 
awaitEE "
	_mediatorEE# ,
.EE, -
SendEE- 1
(EE1 2
requestEE2 9
,EE9 :
cancellationTokenEE; L
)EEL M
;EEM N
returnFF 
resultFF 
;FF 
}GG 
catchHH 
(HH 
	ExceptionHH 
exHH 
)HH  
{II 
SystemJJ 
.JJ 
DiagnosticsJJ "
.JJ" #
DebugJJ# (
.JJ( )
	WriteLineJJ) 2
(JJ2 3
exJJ3 5
.JJ5 6
MessageJJ6 =
)JJ= >
;JJ> ?
throwKK 
;KK 
}LL 
}MM 	
)MM	 

.MM
 
WithMetadataMM 
(MM 
newMM %
SwaggerOperationAttributeMM 5
(MM5 6
$strMM6 E
,MME F
$strMMG i
)MMi j
{MMk l
TagsMMm q
=MMr s
newMMt w
[MMw x
]MMx y
{MMz {
$str	MM| á
}
MMà â
}
MMä ã
)
MMã å
;
MMå ç
appOO 
.OO 
MapGetOO 
(OO 
$strOO &
,OO& '
[OO( )
	AuthorizeOO) 2
(OO2 3
PolicyOO3 9
=OO: ;
$strOO< F
)OOF G
]OOG H
asyncOOI N
(OOO P
stringOOP V
idOOW Y
,OOY Z
CancellationTokenOO[ l
cancellationTokenOOm ~
,OO~ 
ISender
OOÄ á
	_mediator
OOà ë
)
OOë í
=>
OOì ï
{PP 	
tryQQ 
{RR "
GetReferralByIdCommandSS &
requestSS' .
=SS/ 0
newSS1 4
(SS4 5
idSS5 7
)SS7 8
;SS8 9
varTT 
resultTT 
=TT 
awaitTT "
	_mediatorTT# ,
.TT, -
SendTT- 1
(TT1 2
requestTT2 9
,TT9 :
cancellationTokenTT; L
)TTL M
;TTM N
returnUU 
resultUU 
;UU 
}VV 
catchWW 
(WW 
	ExceptionWW 
exWW 
)WW  
{XX 
SystemYY 
.YY 
DiagnosticsYY "
.YY" #
DebugYY# (
.YY( )
	WriteLineYY) 2
(YY2 3
exYY3 5
.YY5 6
MessageYY6 =
)YY= >
;YY> ?
throwZZ 
;ZZ 
}[[ 
}\\ 	
)\\	 

.\\
 
WithMetadata\\ 
(\\ 
new\\ %
SwaggerOperationAttribute\\ 5
(\\5 6
$str\\6 E
,\\E F
$str\\G [
)\\[ \
{\\] ^
Tags\\_ c
=\\d e
new\\f i
[\\i j
]\\j k
{\\l m
$str\\n y
}\\z {
}\\| }
)\\} ~
;\\~ 
app^^ 
.^^ 
MapPost^^ 
(^^ 
$str^^ >
,^^> ?
[^^@ A
	Authorize^^A J
(^^J K
Policy^^K Q
=^^R S
$str^^T ^
)^^^ _
]^^_ `
async^^a f
(^^g h
string^^h n

referralId^^o y
,^^y z
string	^^{ Å
status
^^Ç à
,
^^à â
CancellationToken
^^ä õ
cancellationToken
^^ú ≠
,
^^≠ Æ
ISender
^^Ø ∂
	_mediator
^^∑ ¿
,
^^¿ ¡
ILogger
^^¬ …
<
^^…  &
MinimalReferralEndPoints
^^  ‚
>
^^‚ „
logger
^^‰ Í
)
^^Í Î
=>
^^Ï Ó
{__ 	
try`` 
{aa $
SetReferralStatusCommandbb (
commandbb) 0
=bb1 2
newbb3 6
(bb6 7

referralIdbb7 A
,bbA B
statusbbC I
)bbI J
;bbJ K
varcc 
resultcc 
=cc 
awaitcc "
	_mediatorcc# ,
.cc, -
Sendcc- 1
(cc1 2
commandcc2 9
,cc9 :
cancellationTokencc; L
)ccL M
;ccM N
returndd 
resultdd 
;dd 
}ee 
catchff 
(ff 
	Exceptionff 
exff 
)ff  
{gg 
loggerhh 
.hh 
LogErrorhh 
(hh  
exhh  "
,hh" #
$strhh$ i
,hhi j
exhhk m
.hhm n
Messagehhn u
)hhu v
;hhv w
Systemii 
.ii 
Diagnosticsii "
.ii" #
Debugii# (
.ii( )
	WriteLineii) 2
(ii2 3
exii3 5
.ii5 6
Messageii6 =
)ii= >
;ii> ?
throwjj 
;jj 
}kk 
}ll 	
)ll	 

.ll
 
WithMetadatall 
(ll 
newll %
SwaggerOperationAttributell 5
(ll5 6
$strll6 A
,llA B
$strllC X
)llX Y
{llZ [
Tagsll\ `
=lla b
newllc f
[llf g
]llg h
{lli j
$strllk v
}llw x
}lly z
)llz {
;ll{ |
}mm 
}nn ﬁ
\C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Endpoints\WebMarker.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
	Endpoints% .
;. /
public 
class 
	WebMarker 
{ 
} √Q
uC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Migrations\20221114085529_CreateIntialSchema.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %

Migrations% /
{ 
public 

partial 
class 
CreateIntialSchema +
:, -
	Migration. 7
{		 
	protected

 
override

 
void

 
Up

  "
(

" #
MigrationBuilder

# 3
migrationBuilder

4 D
)

D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str !
,! "
columns 
: 
table 
=> !
new" %
{ 
Id 
= 
table 
. 
Column %
<% &
string& ,
>, -
(- .
type. 2
:2 3
$str4 :
,: ;
nullable< D
:D E
falseF K
)K L
,L M
OrganisationId "
=# $
table% *
.* +
Column+ 1
<1 2
string2 8
>8 9
(9 :
type: >
:> ?
$str@ F
,F G
nullableH P
:P Q
falseR W
)W X
,X Y
	ServiceId 
= 
table  %
.% &
Column& ,
<, -
string- 3
>3 4
(4 5
type5 9
:9 :
$str; R
,R S
	maxLengthT ]
:] ^
$num_ a
,a b
nullablec k
:k l
falsem r
)r s
,s t
ServiceName 
=  !
table" '
.' (
Column( .
<. /
string/ 5
>5 6
(6 7
type7 ;
:; <
$str= C
,C D
nullableE M
:M N
falseO T
)T U
,U V
ServiceDescription &
=' (
table) .
.. /
Column/ 5
<5 6
string6 <
>< =
(= >
type> B
:B C
$strD J
,J K
nullableL T
:T U
falseV [
)[ \
,\ ]
ServiceAsJson !
=" #
table$ )
.) *
Column* 0
<0 1
string1 7
>7 8
(8 9
type9 =
:= >
$str? E
,E F
nullableG O
:O P
falseQ V
)V W
,W X
Referrer 
= 
table $
.$ %
Column% +
<+ ,
string, 2
>2 3
(3 4
type4 8
:8 9
$str: @
,@ A
nullableB J
:J K
falseL Q
)Q R
,R S
FullName 
= 
table $
.$ %
Column% +
<+ ,
string, 2
>2 3
(3 4
type4 8
:8 9
$str: @
,@ A
nullableB J
:J K
falseL Q
)Q R
,R S
HasSpecialNeeds #
=$ %
table& +
.+ ,
Column, 2
<2 3
string3 9
>9 :
(: ;
type; ?
:? @
$strA G
,G H
nullableI Q
:Q R
falseS X
)X Y
,Y Z
Email 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 =
,= >
nullable? G
:G H
falseI N
)N O
,O P
Phone 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 =
,= >
nullable? G
:G H
falseI N
)N O
,O P
ReasonForSupport $
=% &
table' ,
., -
Column- 3
<3 4
string4 :
>: ;
(; <
type< @
:@ A
$strB H
,H I
nullableJ R
:R S
falseT Y
)Y Z
,Z [
Created 
= 
table #
.# $
Column$ *
<* +
DateTime+ 3
>3 4
(4 5
type5 9
:9 :
$str; U
,U V
nullableW _
:_ `
falsea f
)f g
,g h
	CreatedBy 
= 
table  %
.% &
Column& ,
<, -
string- 3
>3 4
(4 5
type5 9
:9 :
$str; S
,S T
	maxLengthU ^
:^ _
$num` c
,c d
nullablee m
:m n
falseo t
)t u
,u v
LastModified  
=! "
table# (
.( )
Column) /
</ 0
DateTime0 8
>8 9
(9 :
type: >
:> ?
$str@ Z
,Z [
nullable\ d
:d e
truef j
)j k
,k l
LastModifiedBy "
=# $
table% *
.* +
Column+ 1
<1 2
string2 8
>8 9
(9 :
type: >
:> ?
$str@ F
,F G
nullableH P
:P Q
trueR V
)V W
}   
,   
constraints!! 
:!! 
table!! "
=>!!# %
{"" 
table## 
.## 

PrimaryKey## $
(##$ %
$str##% 3
,##3 4
x##5 6
=>##7 9
x##: ;
.##; <
Id##< >
)##> ?
;##? @
}$$ 
)$$ 
;$$ 
migrationBuilder&& 
.&& 
CreateTable&& (
(&&( )
name'' 
:'' 
$str'' (
,''( )
columns(( 
:(( 
table(( 
=>(( !
new((" %
{)) 
Id** 
=** 
table** 
.** 
Column** %
<**% &
string**& ,
>**, -
(**- .
type**. 2
:**2 3
$str**4 :
,**: ;
nullable**< D
:**D E
false**F K
)**K L
,**L M
Status++ 
=++ 
table++ "
.++" #
Column++# )
<++) *
string++* 0
>++0 1
(++1 2
type++2 6
:++6 7
$str++8 >
,++> ?
nullable++@ H
:++H I
false++J O
)++O P
,++P Q

ReferralId,, 
=,,  
table,,! &
.,,& '
Column,,' -
<,,- .
string,,. 4
>,,4 5
(,,5 6
type,,6 :
:,,: ;
$str,,< B
,,,B C
nullable,,D L
:,,L M
true,,N R
),,R S
,,,S T
Created-- 
=-- 
table-- #
.--# $
Column--$ *
<--* +
DateTime--+ 3
>--3 4
(--4 5
type--5 9
:--9 :
$str--; U
,--U V
nullable--W _
:--_ `
true--a e
)--e f
,--f g
	CreatedBy.. 
=.. 
table..  %
...% &
Column..& ,
<.., -
string..- 3
>..3 4
(..4 5
type..5 9
:..9 :
$str..; A
,..A B
nullable..C K
:..K L
true..M Q
)..Q R
,..R S
LastModified//  
=//! "
table//# (
.//( )
Column//) /
</// 0
DateTime//0 8
>//8 9
(//9 :
type//: >
://> ?
$str//@ Z
,//Z [
nullable//\ d
://d e
true//f j
)//j k
,//k l
LastModifiedBy00 "
=00# $
table00% *
.00* +
Column00+ 1
<001 2
string002 8
>008 9
(009 :
type00: >
:00> ?
$str00@ F
,00F G
nullable00H P
:00P Q
true00R V
)00V W
}11 
,11 
constraints22 
:22 
table22 "
=>22# %
{33 
table44 
.44 

PrimaryKey44 $
(44$ %
$str44% :
,44: ;
x44< =
=>44> @
x44A B
.44B C
Id44C E
)44E F
;44F G
table55 
.55 

ForeignKey55 $
(55$ %
name66 
:66 
$str66 H
,66H I
column77 
:77 
x77  !
=>77" $
x77% &
.77& '

ReferralId77' 1
,771 2
principalTable88 &
:88& '
$str88( 3
,883 4
principalColumn99 '
:99' (
$str99) -
)99- .
;99. /
}:: 
):: 
;:: 
migrationBuilder<< 
.<< 
CreateIndex<< (
(<<( )
name== 
:== 
$str== 6
,==6 7
table>> 
:>> 
$str>> )
,>>) *
column?? 
:?? 
$str?? $
)??$ %
;??% &
}@@ 	
	protectedBB 
overrideBB 
voidBB 
DownBB  $
(BB$ %
MigrationBuilderBB% 5
migrationBuilderBB6 F
)BBF G
{CC 	
migrationBuilderDD 
.DD 
	DropTableDD &
(DD& '
nameEE 
:EE 
$strEE (
)EE( )
;EE) *
migrationBuilderGG 
.GG 
	DropTableGG &
(GG& '
nameHH 
:HH 
$strHH !
)HH! "
;HH" #
}II 	
}JJ 
}KK ˇ<
ÉC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Migrations\20221215150932_AddTextAndRejectionReasonColumns.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %

Migrations% /
{ 
public 

partial 
class ,
 AddTextAndRejectionReasonColumns 9
:: ;
	Migration< E
{ 
	protected		 
override		 
void		 
Up		  "
(		" #
MigrationBuilder		# 3
migrationBuilder		4 D
)		D E
{

 	
migrationBuilder 
. 
DropForeignKey +
(+ ,
name 
: 
$str @
,@ A
table 
: 
$str )
)) *
;* +
migrationBuilder 
. 
AlterColumn (
<( )
string) /
>/ 0
(0 1
name 
: 
$str "
," #
table 
: 
$str )
,) *
type 
: 
$str 
, 
nullable 
: 
false 
,  
defaultValue 
: 
$str  
,  !

oldClrType 
: 
typeof "
(" #
string# )
)) *
,* +
oldType 
: 
$str 
,  
oldNullable 
: 
true !
)! "
;" #
migrationBuilder 
. 
AlterColumn (
<( )
string) /
>/ 0
(0 1
name 
: 
$str 
, 
table 
: 
$str "
," #
type 
: 
$str 
, 
nullable 
: 
true 
, 

oldClrType 
: 
typeof "
(" #
string# )
)) *
,* +
oldType 
: 
$str 
)  
;  !
migrationBuilder!! 
.!! 
AlterColumn!! (
<!!( )
string!!) /
>!!/ 0
(!!0 1
name"" 
:"" 
$str"" 
,"" 
table## 
:## 
$str## "
,##" #
type$$ 
:$$ 
$str$$ 
,$$ 
nullable%% 
:%% 
true%% 
,%% 

oldClrType&& 
:&& 
typeof&& "
(&&" #
string&&# )
)&&) *
,&&* +
oldType'' 
:'' 
$str'' 
)''  
;''  !
migrationBuilder)) 
.)) 
	AddColumn)) &
<))& '
string))' -
>))- .
()). /
name** 
:** 
$str** *
,*** +
table++ 
:++ 
$str++ "
,++" #
type,, 
:,, 
$str,, 
,,, 
nullable-- 
:-- 
true-- 
)-- 
;--  
migrationBuilder// 
.// 
	AddColumn// &
<//& '
string//' -
>//- .
(//. /
name00 
:00 
$str00 
,00 
table11 
:11 
$str11 "
,11" #
type22 
:22 
$str22 
,22 
nullable33 
:33 
true33 
)33 
;33  
migrationBuilder55 
.55 
AddForeignKey55 *
(55* +
name66 
:66 
$str66 @
,66@ A
table77 
:77 
$str77 )
,77) *
column88 
:88 
$str88 $
,88$ %
principalTable99 
:99 
$str99  +
,99+ ,
principalColumn:: 
:::  
$str::! %
,::% &
onDelete;; 
:;; 
ReferentialAction;; +
.;;+ ,
Cascade;;, 3
);;3 4
;;;4 5
}<< 	
	protected>> 
override>> 
void>> 
Down>>  $
(>>$ %
MigrationBuilder>>% 5
migrationBuilder>>6 F
)>>F G
{?? 	
migrationBuilder@@ 
.@@ 
DropForeignKey@@ +
(@@+ ,
nameAA 
:AA 
$strAA @
,AA@ A
tableBB 
:BB 
$strBB )
)BB) *
;BB* +
migrationBuilderDD 
.DD 

DropColumnDD '
(DD' (
nameEE 
:EE 
$strEE *
,EE* +
tableFF 
:FF 
$strFF "
)FF" #
;FF# $
migrationBuilderHH 
.HH 

DropColumnHH '
(HH' (
nameII 
:II 
$strII 
,II 
tableJJ 
:JJ 
$strJJ "
)JJ" #
;JJ# $
migrationBuilderLL 
.LL 
AlterColumnLL (
<LL( )
stringLL) /
>LL/ 0
(LL0 1
nameMM 
:MM 
$strMM "
,MM" #
tableNN 
:NN 
$strNN )
,NN) *
typeOO 
:OO 
$strOO 
,OO 
nullablePP 
:PP 
truePP 
,PP 

oldClrTypeQQ 
:QQ 
typeofQQ "
(QQ" #
stringQQ# )
)QQ) *
,QQ* +
oldTypeRR 
:RR 
$strRR 
)RR  
;RR  !
migrationBuilderTT 
.TT 
AlterColumnTT (
<TT( )
stringTT) /
>TT/ 0
(TT0 1
nameUU 
:UU 
$strUU 
,UU 
tableVV 
:VV 
$strVV "
,VV" #
typeWW 
:WW 
$strWW 
,WW 
nullableXX 
:XX 
falseXX 
,XX  
defaultValueYY 
:YY 
$strYY  
,YY  !

oldClrTypeZZ 
:ZZ 
typeofZZ "
(ZZ" #
stringZZ# )
)ZZ) *
,ZZ* +
oldType[[ 
:[[ 
$str[[ 
,[[  
oldNullable\\ 
:\\ 
true\\ !
)\\! "
;\\" #
migrationBuilder^^ 
.^^ 
AlterColumn^^ (
<^^( )
string^^) /
>^^/ 0
(^^0 1
name__ 
:__ 
$str__ 
,__ 
table`` 
:`` 
$str`` "
,``" #
typeaa 
:aa 
$straa 
,aa 
nullablebb 
:bb 
falsebb 
,bb  
defaultValuecc 
:cc 
$strcc  
,cc  !

oldClrTypedd 
:dd 
typeofdd "
(dd" #
stringdd# )
)dd) *
,dd* +
oldTypeee 
:ee 
$stree 
,ee  
oldNullableff 
:ff 
trueff !
)ff! "
;ff" #
migrationBuilderhh 
.hh 
AddForeignKeyhh *
(hh* +
nameii 
:ii 
$strii @
,ii@ A
tablejj 
:jj 
$strjj )
,jj) *
columnkk 
:kk 
$strkk $
,kk$ %
principalTablell 
:ll 
$strll  +
,ll+ ,
principalColumnmm 
:mm  
$strmm! %
)mm% &
;mm& '
}nn 	
}oo 
}pp â
PC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Program.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
public 
class 
Program 
{ 
public 

static 
IServiceProvider "
ServiceProvider# 2
{3 4
get5 8
;8 9
private: A
setB E
;E F
}G H
=I J
defaultK R
!R S
;S T
public		 

static		 
async		 
Task		 
Main		 !
(		! "
string		" (
[		( )
]		) *
args		+ /
)		/ 0
{

 
Log 
. 
Logger 
= 
new 
LoggerConfiguration ,
(, -
)- .
. 
WriteTo 
. 
Console 
( 
) 
. !
CreateBootstrapLogger "
(" #
)# $
;$ %
Log 
. 
Information 
( 
$str %
)% &
;& '
try 
{ 	
var 
builder 
= 
WebApplication (
.( )
CreateBuilder) 6
(6 7
args7 ;
); <
;< =
builder 
. 
ConfigureHost !
(! "
)" #
;# $
builder 
. 
Services 
. 
ConfigureServices .
(. /
builder/ 6
.6 7
Configuration7 D
,D E
builderF M
.M N
EnvironmentN Y
.Y Z
IsProductionZ f
(f g
)g h
)h i
;i j
var 
app 
= 
builder 
. 
Build #
(# $
)$ %
;% &
ServiceProvider 
= 
await #
app$ '
.' (#
ConfigureWebApplication( ?
(? @
)@ A
;A B
await 
app 
. 
RunAsync 
( 
)  
;  !
} 	
catch 
( 
	Exception 
e 
) 
{   	
Log!! 
.!! 
Fatal!! 
(!! 
e!! 
,!! 
$str!! O
)!!O P
;!!P Q
}"" 	
finally## 
{$$ 	
Log%% 
.%% 
CloseAndFlush%% 
(%% 
)%% 
;%%  
}&& 	
}'' 
}(( £%
tC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralByIdCommand.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Queries% ,
., -
GetReferrals- 9
;9 :
public

 
class

 "
GetReferralByIdCommand

 #
:

$ %
IRequest

& .
<

. /
ReferralDto

/ :
>

: ;
{ 
public 
"
GetReferralByIdCommand !
(! "
string" (
id) +
)+ ,
{ 
Id 

= 
id 
; 
} 
public 

string 
Id 
{ 
get 
; 
set 
;  
}! "
} 
public 
class )
GetReferralByIdCommandHandler *
:+ ,
IRequestHandler- <
<< ="
GetReferralByIdCommand= S
,S T
ReferralDtoU `
>` a
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
public 
)
GetReferralByIdCommandHandler (
(( ) 
ApplicationDbContext) =
context> E
)E F
{ 
_context 
= 
context 
; 
} 
public 

async 
Task 
< 
ReferralDto !
>! "
Handle# )
() *"
GetReferralByIdCommand* @
requestA H
,H I
CancellationTokenJ [
cancellationToken\ m
)m n
{ 
var 
entity 
= 
await 
_context #
.# $
	Referrals$ -
.   
Include   
(   
x   
=>   
x   
.   
Status   "
)  " #
.!! 
FirstOrDefaultAsync!!  
(!!  !
p!!! "
=>!!# %
p!!& '
.!!' (
Id!!( *
==!!+ -
request!!. 5
.!!5 6
Id!!6 8
,!!8 9
cancellationToken!!: K
:!!K L
cancellationToken!!M ^
)!!^ _
;!!_ `
if## 

(## 
entity## 
==## 
null## 
)## 
{$$ 	
throw%% 
new%% 
NotFoundException%% '
(%%' (
nameof%%( .
(%%. /
Referral%%/ 7
)%%7 8
,%%8 9
request%%: A
.%%A B
Id%%B D
)%%D E
;%%E F
}&& 	
List(( 
<(( 
ReferralStatusDto(( 
>(( 
referralStatusDtos((  2
=((3 4
new((5 8
(((8 9
)((9 :
;((: ;
foreach)) 
()) 
var)) 
status)) 
in)) 
entity)) %
.))% &
Status))& ,
))), -
{** 	
referralStatusDtos++ 
.++ 
Add++ "
(++" #
new++# &
ReferralStatusDto++' 8
(++8 9
status++9 ?
.++? @
Id++@ B
,++B C
status++D J
.++J K
Status++K Q
)++Q R
)++R S
;++S T
},, 	
var.. 
result.. 
=.. 
new.. 
ReferralDto.. $
(..$ %
entity// 
.// 
Id// 
,// 
entity00 
.00 
OrganisationId00  
,00  !
entity11 
.11 
	ServiceId11 
,11 
entity22 
.22 
ServiceName22 
,22 
entity33 
.33 
ServiceDescription33 $
,33$ %
entity44 
.44 
ServiceAsJson44 
,44  
entity55 
.55 
Referrer55 
,55 
entity66 
.66 
FullName66 
,66 
entity77 
.77 
HasSpecialNeeds77 !
,77! "
entity88 
.88 
Email88 
,88 
entity99 
.99 
Phone99 
,99 
entity:: 
.:: 
Text:: 
,:: 
entity;; 
.;; 
ReasonForSupport;; "
,;;" #
entity<< 
.<< 
ReasonForRejection<< $
,<<$ %
referralStatusDtos== 
)>> 
;>> 
return@@ 
result@@ 
;@@ 
}AA 
}BB Ω
}C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralByIdCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Queries% ,
., -
GetReferrals- 9
;9 :
public 
class +
GetReferralByIdCommandValidator ,
:- .
AbstractValidator/ @
<@ A"
GetReferralByIdCommandA W
>W X
{ 
public 
+
GetReferralByIdCommandValidator *
(* +
)+ ,
{ 
RuleFor		 
(		 
v		 
=>		 
v		 
.		 
Id		 
)		 
.

 
NotNull

 
(

 
)

 
. 
NotEmpty 
( 
) 
; 
} 
} Ï8
ÅC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralsByOrganisationIdCommand.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Api		! $
.		$ %
Queries		% ,
.		, -
GetReferrals		- 9
;		9 :
public 
class /
#GetReferralsByOrganisationIdCommand 0
:1 2
IRequest3 ;
<; <
PaginatedList< I
<I J
ReferralDtoJ U
>U V
>V W
{ 
public 
/
#GetReferralsByOrganisationIdCommand .
(. /
string/ 5
organisationId6 D
,D E
intF I
?I J

pageNumberK U
,U V
intW Z
?Z [
pageSize\ d
)d e
{ 
OrganisationId 
= 
organisationId '
;' (

PageNumber 
= 

pageNumber 
!=  "
null# '
?( )

pageNumber* 4
.4 5
Value5 :
:; <
$num= >
;> ?
PageSize 
= 
pageSize 
!= 
null #
?$ %
pageSize& .
.. /
Value/ 4
:5 6
$num7 8
;8 9
} 
public 

string 
OrganisationId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 

PageNumber 
{ 
get 
;  
set! $
;$ %
}& '
=( )
$num* +
;+ ,
public 

int 
PageSize 
{ 
get 
; 
set "
;" #
}$ %
=& '
$num( *
;* +
} 
public 
class 6
*GetReferralsByOrganisationIdCommandHandler 7
:8 9
IRequestHandler: I
<I J/
#GetReferralsByOrganisationIdCommandJ m
,m n
PaginatedListo |
<| }
ReferralDto	} à
>
à â
>
â ä
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
public 
6
*GetReferralsByOrganisationIdCommandHandler 5
(5 6 
ApplicationDbContext6 J
contextK R
)R S
{ 
_context 
= 
context 
; 
}   
public!! 

async!! 
Task!! 
<!! 
PaginatedList!! #
<!!# $
ReferralDto!!$ /
>!!/ 0
>!!0 1
Handle!!2 8
(!!8 9/
#GetReferralsByOrganisationIdCommand!!9 \
request!!] d
,!!d e
CancellationToken!!f w
cancellationToken	!!x â
)
!!â ä
{"" 
var## 
entities## 
=## 
_context## 
.##  
	Referrals##  )
.$$ 
Include$$ 
($$ 
x$$ 
=>$$ 
x$$ 
.$$ 
Status$$ "
)$$" #
.%% 
Where%% 
(%% 
x%% 
=>%% 
x%% 
.%% 
OrganisationId%% (
==%%) +
request%%, 3
.%%3 4
OrganisationId%%4 B
)%%B C
;%%C D
if'' 

('' 
entities'' 
=='' 
null'' 
)'' 
{(( 	
throw)) 
new)) 
NotFoundException)) '
())' (
nameof))( .
()). /
Referral))/ 7
)))7 8
,))8 9
request)): A
.))A B
OrganisationId))B P
)))P Q
;))Q R
}** 	
var,, 
filteredReferrals,, 
=,, 
await,,  %
entities,,& .
.,,. /
Select,,/ 5
(,,5 6
x,,6 7
=>,,8 :
new,,; >
ReferralDto,,? J
(,,J K
x-- 
.-- 
Id-- 
,-- 
x.. 
... 
OrganisationId.. 
,.. 
x// 
.// 
	ServiceId// 
,// 
x00 
.00 
ServiceName00 
,00 
x11 
.11 
ServiceDescription11  
,11  !
x22 
.22 
ServiceAsJson22 
,22 
x33 
.33 
Referrer33 
,33 
x44 
.44 
FullName44 
,44 
x55 
.55 
HasSpecialNeeds55 
,55 
x66 
.66 
Email66 
,66 
x77 
.77 
Phone77 
,77 
x88 
.88 
Text88 
,88 
x99 
.99 
ReasonForSupport99 
,99 
x:: 
.:: 
ReasonForRejection::  
,::  !
x;; 
.;; 
Status;; 
.;; 
Select;; 
(;; 
x;; 
=>;;  
new;;! $
ReferralStatusDto;;% 6
(;;6 7
x;;7 8
.;;8 9
Id;;9 ;
,;;; <
x;;= >
.;;> ?
Status;;? E
);;E F
);;F G
.;;G H
ToList;;H N
(;;N O
);;O P
)<< 
)<< 
.<< 
ToListAsync<< 
(<< 
)<< 
;<< 
if>> 

(>> 
request>> 
!=>> 
null>> 
)>> 
{?? 	
var@@ 
pagelist@@ 
=@@ 
filteredReferrals@@ ,
.@@, -
Skip@@- 1
(@@1 2
(@@2 3
request@@3 :
.@@: ;

PageNumber@@; E
-@@F G
$num@@H I
)@@I J
*@@K L
request@@M T
.@@T U
PageSize@@U ]
)@@] ^
.@@^ _
Take@@_ c
(@@c d
request@@d k
.@@k l
PageSize@@l t
)@@t u
.@@u v
ToList@@v |
(@@| }
)@@} ~
;@@~ 
varAA 
resultAA 
=AA 
newAA 
PaginatedListAA *
<AA* +
ReferralDtoAA+ 6
>AA6 7
(AA7 8
filteredReferralsAA8 I
,AAI J
pagelistAAK S
.AAS T
CountAAT Y
,AAY Z
requestAA[ b
.AAb c

PageNumberAAc m
,AAm n
requestAAo v
.AAv w
PageSizeAAw 
)	AA Ä
;
AAÄ Å
returnBB 
resultBB 
;BB 
}CC 	
returnEE 
newEE 
PaginatedListEE  
<EE  !
ReferralDtoEE! ,
>EE, -
(EE- .
filteredReferralsEE. ?
,EE? @
filteredReferralsEEA R
.EER S
CountEES X
,EEX Y
$numEEZ [
,EE[ \
$numEE] _
)EE_ `
;EE` a
}HH 
}II ˛
äC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralsByOrganisationIdCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Queries% ,
., -
GetReferrals- 9
;9 :
public 
class 8
,GetReferralsByOrganisationIdCommandValidator 9
:: ;
AbstractValidator< M
<M N/
#GetReferralsByOrganisationIdCommandN q
>q r
{ 
public 
8
,GetReferralsByOrganisationIdCommandValidator 7
(7 8
)8 9
{ 
RuleFor		 
(		 
v		 
=>		 
v		 
.		 
OrganisationId		 %
)		% &
.

 
NotNull

 
(

 
)

 
. 
NotEmpty 
( 
) 
; 
} 
} ò8
{C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralsByReferrerCommand.cs
	namespace		 	

FamilyHubs		
 
.		 
ReferralApi		  
.		  !
Api		! $
.		$ %
Queries		% ,
.		, -
GetReferrals		- 9
;		9 :
public 
class )
GetReferralsByReferrerCommand *
:+ ,
IRequest- 5
<5 6
PaginatedList6 C
<C D
ReferralDtoD O
>O P
>P Q
{ 
public 
)
GetReferralsByReferrerCommand (
(( )
string) /
referrer0 8
,8 9
int: =
?= >

pageNumber? I
,I J
intK N
?N O
pageSizeP X
)X Y
{ 
Referrer 
= 
referrer 
; 

PageNumber 
= 

pageNumber 
!=  "
null# '
?( )

pageNumber* 4
.4 5
Value5 :
:; <
$num= >
;> ?
PageSize 
= 
pageSize 
!= 
null #
?$ %
pageSize& .
.. /
Value/ 4
:5 6
$num7 8
;8 9
} 
public 

string 
Referrer 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 

PageNumber 
{ 
get 
;  
set! $
;$ %
}& '
=( )
$num* +
;+ ,
public 

int 
PageSize 
{ 
get 
; 
set "
;" #
}$ %
=& '
$num( *
;* +
} 
public 
class 0
$GetReferralsByReferrerCommandHandler 1
:2 3
IRequestHandler4 C
<C D)
GetReferralsByReferrerCommandD a
,a b
PaginatedListc p
<p q
ReferralDtoq |
>| }
>} ~
{ 
private 
readonly  
ApplicationDbContext )
_context* 2
;2 3
public 
0
$GetReferralsByReferrerCommandHandler /
(/ 0 
ApplicationDbContext0 D
contextE L
)L M
{ 
_context   
=   
context   
;   
}!! 
public"" 

async"" 
Task"" 
<"" 
PaginatedList"" #
<""# $
ReferralDto""$ /
>""/ 0
>""0 1
Handle""2 8
(""8 9)
GetReferralsByReferrerCommand""9 V
request""W ^
,""^ _
CancellationToken""` q
cancellationToken	""r É
)
""É Ñ
{## 
var$$ 
entities$$ 
=$$ 
_context$$ 
.$$  
	Referrals$$  )
.%% 
Include%% 
(%% 
x%% 
=>%% 
x%% 
.%% 
Status%% "
)%%" #
.&& 
Where&& 
(&& 
x&& 
=>&& 
x&& 
.&& 
Referrer&& "
==&&# %
request&&& -
.&&- .
Referrer&&. 6
)&&6 7
;&&7 8
if(( 

((( 
entities(( 
==(( 
null(( 
)(( 
{)) 	
throw** 
new** 
NotFoundException** '
(**' (
nameof**( .
(**. /
Referral**/ 7
)**7 8
,**8 9
request**: A
.**A B
Referrer**B J
)**J K
;**K L
}++ 	
var-- 
filteredReferrals-- 
=-- 
await--  %
entities--& .
.--. /
Select--/ 5
(--5 6
x--6 7
=>--8 :
new--; >
ReferralDto--? J
(--J K
x.. 
... 
Id.. 
,.. 
x// 
.// 
OrganisationId// 
,// 
x00 
.00 
	ServiceId00 
,00 
x11 
.11 
ServiceName11 
,11 
x22 
.22 
ServiceDescription22  
,22  !
x33 
.33 
ServiceAsJson33 
,33 
x44 
.44 
Referrer44 
,44 
x55 
.55 
FullName55 
,55 
x66 
.66 
HasSpecialNeeds66 
,66 
x77 
.77 
Email77 
,77 
x88 
.88 
Phone88 
,88 
string99 
.99 
Empty99 
,99 
x:: 
.:: 
ReasonForSupport:: 
,:: 
x;; 
.;; 
ReasonForRejection;;  
,;;  !
x<< 
.<< 
Status<< 
.<< 
Select<< 
(<< 
x<< 
=><<  
new<<! $
ReferralStatusDto<<% 6
(<<6 7
x<<7 8
.<<8 9
Id<<9 ;
,<<; <
x<<= >
.<<> ?
Status<<? E
)<<E F
)<<F G
.<<G H
ToList<<H N
(<<N O
)<<O P
)== 
)== 
.== 
ToListAsync== 
(== 
)== 
;== 
if?? 

(?? 
request?? 
!=?? 
null?? 
)?? 
{@@ 	
varAA 
pagelistAA 
=AA 
filteredReferralsAA ,
.AA, -
SkipAA- 1
(AA1 2
(AA2 3
requestAA3 :
.AA: ;

PageNumberAA; E
-AAF G
$numAAH I
)AAI J
*AAK L
requestAAM T
.AAT U
PageSizeAAU ]
)AA] ^
.AA^ _
TakeAA_ c
(AAc d
requestAAd k
.AAk l
PageSizeAAl t
)AAt u
.AAu v
ToListAAv |
(AA| }
)AA} ~
;AA~ 
varBB 
resultBB 
=BB 
newBB 
PaginatedListBB *
<BB* +
ReferralDtoBB+ 6
>BB6 7
(BB7 8
filteredReferralsBB8 I
,BBI J
pagelistBBK S
.BBS T
CountBBT Y
,BBY Z
requestBB[ b
.BBb c

PageNumberBBc m
,BBm n
requestBBo v
.BBv w
PageSizeBBw 
)	BB Ä
;
BBÄ Å
returnCC 
resultCC 
;CC 
}DD 	
returnFF 
newFF 
PaginatedListFF  
<FF  !
ReferralDtoFF! ,
>FF, -
(FF- .
filteredReferralsFF. ?
,FF? @
filteredReferralsFFA R
.FFR S
CountFFS X
,FFX Y
$numFFZ [
,FF[ \
$numFF] _
)FF_ `
;FF` a
}II 
}JJ ‡
ÑC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\Queries\GetReferrals\GetReferralsByReferrerCommandValidator.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
.$ %
Queries% ,
., -
GetReferrals- 9
;9 :
public 
class 2
&GetReferralsByReferrerCommandValidator 3
:4 5
AbstractValidator6 G
<G H)
GetReferralsByReferrerCommandH e
>e f
{ 
public 
2
&GetReferralsByReferrerCommandValidator 1
(1 2
)2 3
{ 
RuleFor		 
(		 
v		 
=>		 
v		 
.		 
Referrer		 
)		  
.

 
NotNull

 
(

 
)

 
. 
NotEmpty 
( 
) 
; 
} 
} â
YC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\RabbitMqSettings.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
[ #
ExcludeFromCodeCoverage 
] 
public 
class 
RabbitMqSettings 
{ 
public 

string 
Uri 
{ 
get 
; 
set  
;  !
}" #
=$ %
null& *
!* +
;+ ,
public		 

string		 
UserName		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
=		) *
null		+ /
!		/ 0
;		0 1
public

 

string

 
Password

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
=

) *
null

+ /
!

/ 0
;

0 1
} ®f
ZC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Api\StartupExtensions.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Api! $
;$ %
public 
static 
class 
StartupExtensions %
{ 
public 

static 
void 
ConfigureHost $
($ %
this% )!
WebApplicationBuilder* ?
builder@ G
)G H
{ 
builder 
. 
Host 
. 

UseSerilog 
(  
(  !
_! "
," #
services$ ,
,, -
loggerConfiguration. A
)A B
=>C E
{ 	
var 
logLevelString 
=  
builder! (
.( )
Configuration) 6
[6 7
$str7 A
]A B
;B C
var 
parsed 
= 
Enum 
. 
TryParse &
<& '
LogEventLevel' 4
>4 5
(5 6
logLevelString6 D
,D E
outF I
varJ M
logLevelN V
)V W
;W X
loggerConfiguration 
.  
WriteTo  '
.' (
ApplicationInsights( ;
(; <
services 
. 
GetRequiredService +
<+ ,"
TelemetryConfiguration, B
>B C
(C D
)D E
,E F
TelemetryConverter "
." #
Traces# )
,) *
parsed 
? 
logLevel !
:" #
LogEventLevel$ 1
.1 2
Warning2 9
)9 :
;: ;
loggerConfiguration 
.  
WriteTo  '
.' (
Console( /
(/ 0
parsed 
? 
logLevel !
:" #
LogEventLevel$ 1
.1 2
Warning2 9
)9 :
;: ;
}   	
)  	 

;  
 
}!! 
public## 

static## 
void## 
ConfigureServices## (
(##( )
this##) -
IServiceCollection##. @
services##A I
,##I J
IConfiguration##K Y
configuration##Z g
,##g h
bool##i m
isProduction##n z
)##z {
{$$ 
services%% 
.%% +
AddApplicationInsightsTelemetry%% 0
(%%0 1
)%%1 2
;%%2 3
services(( 
.(( 
AddAuthentication(( "
(((" #
options((# *
=>((+ -
{)) 	
options** 
.** %
DefaultAuthenticateScheme** -
=**. /
JwtBearerDefaults**0 A
.**A B 
AuthenticationScheme**B V
;**V W
options++ 
.++ "
DefaultChallengeScheme++ *
=+++ ,
JwtBearerDefaults++- >
.++> ? 
AuthenticationScheme++? S
;++S T
options,, 
.,, 
DefaultScheme,, !
=,," #
JwtBearerDefaults,,$ 5
.,,5 6 
AuthenticationScheme,,6 J
;,,J K
}-- 	
)--	 

... 	
AddJwtBearer..	 
(.. 
options.. 
=>..  
{// 	
options11 
.11 
	SaveToken11 
=11 
true11  $
;11$ %
options22 
.22  
RequireHttpsMetadata22 (
=22) *
false22+ 0
;220 1
options33 
.33 %
TokenValidationParameters33 -
=33. /
new330 3%
TokenValidationParameters334 M
(33M N
)33N O
{44 
ValidateIssuer55 
=55  
true55! %
,55% &
ValidateAudience66  
=66! "
true66# '
,66' (
ValidAudience77 
=77 
configuration77  -
[77- .
$str77. A
]77A B
,77B C
ValidIssuer88 
=88 
configuration88 +
[88+ ,
$str88, =
]88= >
,88> ?
IssuerSigningKey99  
=99! "
new99# & 
SymmetricSecurityKey99' ;
(99; <
Encoding99< D
.99D E
UTF899E I
.99I J
GetBytes99J R
(99R S
configuration:: !
[::! "
$str::" .
]::. /
??::0 2
$str::3 e
)::e f
)::f g
};; 
;;; 
}<< 	
)<<	 

;<<
 
services?? 
.?? 
AddAuthorization?? !
(??! "
options??" )
=>??* ,
{@@ 	
ifAA 
(AA 
isProductionAA 
)AA 
{BB 
optionsCC 
.CC 
	AddPolicyCC !
(CC! "
$strCC" ,
,CC, -
policyCC. 4
=>CC5 7
policyDD 
.DD 
RequireAssertionDD +
(DD+ ,
contextDD, 3
=>DD4 6
contextEE 
.EE  
UserEE  $
.EE$ %
IsInRoleEE% -
(EE- .
$strEE. 8
)EE8 9
||EE: <
contextFF 
.FF  
UserFF  $
.FF$ %
IsInRoleFF% -
(FF- .
$strFF. <
)FF< =
)FF= >
)FF> ?
;FF? @
}GG 
elseHH 
{II 
optionsJJ 
.JJ 
	AddPolicyJJ !
(JJ! "
$strJJ" ,
,JJ, -
policyJJ. 4
=>JJ5 7
policyKK 
.KK 
RequireAssertionKK +
(KK+ ,
_KK, -
=>KK. 0
trueKK1 5
)KK5 6
)KK6 7
;KK7 8
}LL 
}MM 	
)MM	 

;MM
 
servicesPP 
.PP 
AddControllersPP 
(PP  
)PP  !
;PP! "
servicesRR 
.RR #
AddEndpointsApiExplorerRR (
(RR( )
)RR) *
.SS %
AddInfrastructureServicesSS &
(SS& '
configurationSS' 4
)SS4 5
.TT "
AddApplicationServicesTT #
(TT# $
)TT$ %
;TT% &
servicesVV 
.VV 
AddTransientVV 
<VV #
MinimalGeneralEndPointsVV 5
>VV5 6
(VV6 7
)VV7 8
;VV8 9
servicesWW 
.WW 
AddTransientWW 
<WW $
MinimalReferralEndPointsWW 6
>WW6 7
(WW7 8
)WW8 9
;WW9 :
servicesYY 
.YY 
AddSwaggerGenYY 
(YY 
)YY  
;YY  !
if[[ 

([[ 
![[ 
configuration[[ 
.[[ 
GetValue[[ #
<[[# $
bool[[$ (
>[[( )
([[) *
$str[[* 7
)[[7 8
)[[8 9
return[[: @
;[[@ A
var]] 
rabbitMqSettings]] 
=]] 
configuration]] ,
.]], -

GetSection]]- 7
(]]7 8
nameof]]8 >
(]]> ?
RabbitMqSettings]]? O
)]]O P
)]]P Q
.]]Q R
Get]]R U
<]]U V
RabbitMqSettings]]V f
>]]f g
(]]g h
)]]h i
;]]i j
services^^ 
.^^ 
AddMassTransit^^ 
(^^  
mt^^  "
=>^^# %
mt__ 
.__ 
UsingRabbitMq__ 
(__ 
(__ 
___ 
,__  
cfg__! $
)__$ %
=>__& (
{`` 
cfgaa 
.aa 
Hostaa 
(aa 
rabbitMqSettingsaa )
.aa) *
Uriaa* -
,aa- .
$straa/ 2
,aa2 3
caa4 5
=>aa6 8
{bb 
ccc 
.cc 
Usernamecc 
(cc 
rabbitMqSettingscc /
.cc/ 0
UserNamecc0 8
)cc8 9
;cc9 :
cdd 
.dd 
Passworddd 
(dd 
rabbitMqSettingsdd /
.dd/ 0
Passworddd0 8
)dd8 9
;dd9 :
}ee 
)ee 
;ee 
cfggg 
.gg 
ReceiveEndpointgg #
(gg# $
$strgg$ 3
,gg3 4
(gg5 6
cgg6 7
)gg7 8
=>gg9 ;
{gg< =
cgg> ?
.gg? @
Consumergg@ H
<ggH I"
CommandMessageConsumerggI _
>gg_ `
(gg` a
)gga b
;ggb c
}ggd e
)gge f
;ggf g
}hh 
)hh 
)hh 
;hh 
}ii 
publickk 

statickk 
asynckk 
Taskkk 
<kk 
IServiceProviderkk -
>kk- .#
ConfigureWebApplicationkk/ F
(kkF G
thiskkG K
WebApplicationkkL Z
appkk[ ^
)kk^ _
{ll 
appmm 
.mm $
UseSerilogRequestLoggingmm $
(mm$ %
)mm% &
;mm& '
ifpp 

(pp 
apppp 
.pp 
Environmentpp 
.pp 
IsDevelopmentpp )
(pp) *
)pp* +
)pp+ ,
{qq 	
apprr 
.rr 

UseSwaggerrr 
(rr 
)rr 
;rr 
appss 
.ss 
UseSwaggerUIss 
(ss 
)ss 
;ss 
}tt 	
appvv 
.vv 
UseHttpsRedirectionvv 
(vv  
)vv  !
;vv! "
appxx 
.xx 
UseAuthenticationxx 
(xx 
)xx 
;xx  
appyy 
.yy 
UseAuthorizationyy 
(yy 
)yy 
;yy 
app{{ 
.{{ 
MapControllers{{ 
({{ 
){{ 
;{{ 
await}} 
app}} 
.}} 
RegisterEndPoints}} #
(}}# $
)}}$ %
;}}% &
return 
app 
. 
Services 
; 
}
ÄÄ 
private
ÇÇ 
static
ÇÇ 
async
ÇÇ 
Task
ÇÇ 
RegisterEndPoints
ÇÇ /
(
ÇÇ/ 0
this
ÇÇ0 4
WebApplication
ÇÇ5 C
app
ÇÇD G
)
ÇÇG H
{
ÉÉ 
using
ÑÑ 
var
ÑÑ 
scope
ÑÑ 
=
ÑÑ 
app
ÑÑ 
.
ÑÑ 
Services
ÑÑ &
.
ÑÑ& '
CreateScope
ÑÑ' 2
(
ÑÑ2 3
)
ÑÑ3 4
;
ÑÑ4 5
var
ÜÜ 
genapi
ÜÜ 
=
ÜÜ 
scope
ÜÜ 
.
ÜÜ 
ServiceProvider
ÜÜ *
.
ÜÜ* +

GetService
ÜÜ+ 5
<
ÜÜ5 6%
MinimalGeneralEndPoints
ÜÜ6 M
>
ÜÜM N
(
ÜÜN O
)
ÜÜO P
;
ÜÜP Q
genapi
áá 
?
áá 
.
áá -
RegisterMinimalGeneralEndPoints
áá /
(
áá/ 0
app
áá0 3
)
áá3 4
;
áá4 5
var
ââ 
referralApi
ââ 
=
ââ 
scope
ââ 
.
ââ  
ServiceProvider
ââ  /
.
ââ/ 0

GetService
ââ0 :
<
ââ: ;&
MinimalReferralEndPoints
ââ; S
>
ââS T
(
ââT U
)
ââU V
;
ââV W
referralApi
ää 
?
ää 
.
ää '
RegisterReferralEndPoints
ää .
(
ää. /
app
ää/ 2
)
ää2 3
;
ää3 4
try
åå 
{
çç 	
if
éé 
(
éé 
!
éé 
app
éé 
.
éé 
Environment
éé  
.
éé  !
IsProduction
éé! -
(
éé- .
)
éé. /
)
éé/ 0
{
èè 
var
ëë 
initialiser
ëë 
=
ëë  !
scope
ëë" '
.
ëë' (
ServiceProvider
ëë( 7
.
ëë7 8 
GetRequiredService
ëë8 J
<
ëëJ K-
ApplicationDbContextInitialiser
ëëK j
>
ëëj k
(
ëëk l
)
ëël m
;
ëëm n
await
íí 
initialiser
íí !
.
íí! "
InitialiseAsync
íí" 1
(
íí1 2
app
íí2 5
.
íí5 6
Configuration
íí6 C
)
ííC D
;
ííD E
await
ìì 
initialiser
ìì !
.
ìì! "
	SeedAsync
ìì" +
(
ìì+ ,
)
ìì, -
;
ìì- .
}
îî 
}
ïï 	
catch
ññ 
(
ññ 
	Exception
ññ 
ex
ññ 
)
ññ 
{
óó 	
Log
òò 
.
òò 
Error
òò 
(
òò 
ex
òò 
,
òò 
$str
òò P
,
òòP Q
ex
òòR T
.
òòT U
Message
òòU \
)
òò\ ]
;
òò] ^
}
ôô 	
}
öö 
}õõ 