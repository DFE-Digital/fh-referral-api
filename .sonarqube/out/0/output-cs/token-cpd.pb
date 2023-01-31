•
]C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\AutoMappingProfiles.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
;% &
public 
class 
AutoMappingProfiles  
:! "
Profile# *
{ 
public		 

AutoMappingProfiles		 
(		 
)		  
{

 
	CreateMap 
< 
ReferralDto 
, 
Referral '
>' (
(( )
)) *
;* +
	CreateMap 
< 
ReferralStatusDto #
,# $
ReferralStatus% 3
>3 4
(4 5
)5 6
;6 7
} 
} ù3
[C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Entities\Referral.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &
Entities& .
;. /
public 
class 
Referral 
: 

EntityBase "
<" #
string# )
>) *
,* +
IAggregateRoot, :
{ 
private		 
Referral		 
(		 
)		 
{		 
}		 
public

 

Referral

 
(

 
string

 
id

 
,

 
string

 %
organisationId

& 4
,

4 5
string

6 <
	serviceId

= F
,

F G
string

H N
serviceName

O Z
,

Z [
string

\ b
serviceDescription

c u
,

u v
string

w }
serviceAsJson	

~ ã
,


ã å
string


ç ì
referrer


î ú
,


ú ù
string


û §
fullName


• ≠
,


≠ Æ
string


Ø µ
hasSpecialNeeds


∂ ≈
,


≈ ∆
string


« Õ
?


Õ Œ
email


œ ‘
,


‘ ’
string


÷ ‹
?


‹ ›
phone


ﬁ „
,


„ ‰
string


Â Î
?


Î Ï
text


Ì Ò
,


Ò Ú
string


Û ˘
reasonForSupport


˙ ä
,


ä ã
string


å í
?


í ì 
reasonForRejection


î ¶
,


¶ ß
ICollection


® ≥
<


≥ ¥
ReferralStatus


¥ ¬
>


¬ √
status


ƒ  
)


  À
{ 
Id 

= 
id 
; 
OrganisationId 
= 
organisationId '
;' (
	ServiceId 
= 
	serviceId 
; 
ServiceName 
= 
serviceName !
;! "
ServiceDescription 
= 
serviceDescription /
;/ 0
ServiceAsJson 
= 
serviceAsJson %
;% &
Referrer 
= 
referrer 
; 
FullName 
= 
fullName 
; 
HasSpecialNeeds 
= 
hasSpecialNeeds )
;) *
Email 
= 
email 
; 
Phone 
= 
phone 
; 
Text 
= 
text 
; 
ReasonForSupport 
= 
reasonForSupport +
;+ ,
ReasonForRejection 
= 
reasonForRejection /
;/ 0
Status 
= 
status 
; 
} 
public 

string 
OrganisationId  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
default1 8
!8 9
;9 :
public 

string 
	ServiceId 
{ 
get !
;! "
set# &
;& '
}( )
=* +
default, 3
!3 4
;4 5
public 

string 
ServiceName 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
default. 5
!5 6
;6 7
public 

string 
ServiceDescription $
{% &
get' *
;* +
set, /
;/ 0
}1 2
=3 4
default5 <
!< =
;= >
public   

string   
ServiceAsJson   
{    !
get  " %
;  % &
set  ' *
;  * +
}  , -
=  . /
default  0 7
!  7 8
;  8 9
public!! 

string!! 
Referrer!! 
{!! 
get!!  
;!!  !
set!!" %
;!!% &
}!!' (
=!!) *
default!!+ 2
!!!2 3
;!!3 4
public"" 

string"" 
FullName"" 
{"" 
get""  
;""  !
set""" %
;""% &
}""' (
="") *
default""+ 2
!""2 3
;""3 4
[## 
EncryptColumn## 
]## 
public$$ 

string$$ 
HasSpecialNeeds$$ !
{$$" #
get$$$ '
;$$' (
set$$) ,
;$$, -
}$$. /
=$$0 1
default$$2 9
!$$9 :
;$$: ;
[%% 
EncryptColumn%% 
]%% 
public&& 

string&& 
?&& 
Email&& 
{&& 
get&& 
;&& 
set&&  #
;&&# $
}&&% &
=&&' (
default&&) 0
!&&0 1
;&&1 2
['' 
EncryptColumn'' 
]'' 
public(( 

string(( 
?(( 
Phone(( 
{(( 
get(( 
;(( 
set((  #
;((# $
}((% &
=((' (
default(() 0
!((0 1
;((1 2
[)) 
EncryptColumn)) 
])) 
public** 

string** 
?** 
Text** 
{** 
get** 
;** 
set** "
;**" #
}**$ %
=**& '
default**( /
!**/ 0
;**0 1
[++ 
EncryptColumn++ 
]++ 
public,, 

string,, 
ReasonForSupport,, "
{,,# $
get,,% (
;,,( )
set,,* -
;,,- .
},,/ 0
=,,1 2
default,,3 :
!,,: ;
;,,; <
[-- 
EncryptColumn-- 
]-- 
public.. 

string.. 
?.. 
ReasonForRejection.. %
{..& '
get..( +
;..+ ,
set..- 0
;..0 1
}..2 3
=..4 5
default..6 =
!..= >
;..> ?
public// 

virtual// 
ICollection// 
<// 
ReferralStatus// -
>//- .
Status/// 5
{//6 7
get//8 ;
;//; <
set//= @
;//@ A
}//B C
=//D E
default//F M
!//M N
;//N O
}11 æ
aC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Entities\ReferralStatus.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &
Entities& .
;. /
public 
class 
ReferralStatus 
: 

EntityBase (
<( )
string) /
>/ 0
{ 
private 
ReferralStatus 
( 
) 
{ 
}  
public 

ReferralStatus 
( 
string  
id! #
,# $
string% +
status, 2
,2 3
string4 :

referralId; E
)E F
{		 
Id

 

=

 
id

 
;

 
Status 
= 
status 
; 

ReferralId 
= 

referralId 
;  
} 
public 

string 
Status 
{ 
get 
; 
set  #
;# $
}% &
=' (
default) 0
!0 1
;1 2
public 

string 

ReferralId 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
default- 4
!4 5
;5 6
} §
eC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Events\ReferralCreatedEvent.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &
Events& ,
;, -
public 
class  
ReferralCreatedEvent !
:" #
DomainEventBase$ 3
,3 4!
IReferralCreatedEvent5 J
{ 
public		 
 
ReferralCreatedEvent		 
(		  
Referral		  (
item		) -
)		- .
{

 
Item 
= 
item 
; 
} 
public 

Referral 
Item 
{ 
get 
; 
}  !
} »
kC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Events\ReferralStatusCreatedEvent.cs
	namespace

 	

FamilyHubs


 
.

 
ReferralApi

  
.

  !
Core

! %
.

% &
Events

& ,
;

, -
public 
class &
ReferralStatusCreatedEvent '
:( )
DomainEventBase* 9
,9 :'
IReferralStatusCreatedEvent; V
{ 
public 
&
ReferralStatusCreatedEvent %
(% &
ReferralStatus& 4
item5 9
)9 :
{ 
Item 
= 
item 
; 
} 
public 

ReferralStatus 
Item 
{  
get! $
;$ %
}& '
} §
eC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Events\ReferralUpdatedEvent.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &
Events& ,
;, -
public 
class  
ReferralUpdatedEvent !
:" #
DomainEventBase$ 3
,3 4!
IReferralCreatedEvent5 J
{		 
public

 
 
ReferralUpdatedEvent

 
(

  
Referral

  (
item

) -
)

- .
{ 
Item 
= 
item 
; 
} 
public 

Referral 
Item 
{ 
get 
; 
}  !
} Ω
]C:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\ICurrentUserService.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
;% &
public 
	interface 
ICurrentUserService $
{ 
string 

?
 
UserId 
{ 
get 
; 
} 
} ï
nC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Infrastructure\IApplicationDbContext.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &
Infrastructure& 4
;4 5
public 
	interface !
IApplicationDbContext &
{ 
DbSet 	
<	 

Referral
 
> 
	Referrals 
{ 
get  #
;# $
}% &
DbSet		 	
<			 

ReferralStatus		
 
>		 
ReferralStatuses		 *
{		+ ,
get		- 0
;		0 1
}		2 3
Task

 
<

 	
int

	 
>

 
SaveChangesAsync

 
(

 
CancellationToken

 0
cancellationToken

1 B
)

B C
;

C D
} ≤
tC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Commands\ICreateReferralCommand.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Commands1 9
;9 :
public 
	interface "
ICreateReferralCommand '
{ 
public 

ReferralDto 
ReferralDto "
{# $
get% (
;( )
}* +
} Ñ
wC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Commands\ISetReferralStatusCommand.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Commands1 9
;9 :
public 
	interface %
ISetReferralStatusCommand *
{ 
string 

Status 
{ 
get 
; 
} 
string 


ReferralId 
{ 
get 
; 
} 
} ≤
tC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Commands\IUpdateReferralCommand.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Commands1 9
;9 :
public 
	interface "
IUpdateReferralCommand '
{ 
public 

ReferralDto 
ReferralDto "
{# $
get% (
;( )
}* +
} é
qC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Events\IReferralCreatedEvent.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Events1 7
;7 8
public 
	interface !
IReferralCreatedEvent &
{ 
Referral 
Item 
{ 
get 
; 
} 
} †
wC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Events\IReferralStatusCreatedEvent.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Events1 7
;7 8
public 
	interface '
IReferralStatusCreatedEvent ,
{ 
ReferralStatus 
Item 
{ 
get 
; 
}  
} †
pC:\Projects\FamilyHubs\fh-referral-api\src\FamilyHubs.ReferralApi.Core\Interfaces\Events\IReferralUpdateEvent.cs
	namespace 	

FamilyHubs
 
. 
ReferralApi  
.  !
Core! %
.% &

Interfaces& 0
.0 1
Events1 7
;7 8
public 
	interface  
IReferralUpdateEvent %
{ 
public 

Referral 
Item 
{ 
get 
; 
}  !
} 