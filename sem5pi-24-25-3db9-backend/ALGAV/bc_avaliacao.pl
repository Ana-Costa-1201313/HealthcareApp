
%
%%surgery(SurgeryType,TAnesthesia,TSurgery,TCleaning).
%
%surgery(so2,45,60,45). % 150
%surgery(so3,45,90,45). % 180	
%surgery(so4,45,75,45). % 165
%surgery(so4,45,20,20). % 85
surgery(so2,3,60,3). % 150
surgery(so3,3,90,3). % 180	
surgery(so4,3,20,3). % 85
surgery(so5,3,20,3). % 85

staff(d001	,doctor,orthopaedist,[so2,so3,so4]).
staff(d0011	,doctor,anesthesia	,[so2,so3,so4]).
staff(d0012 ,doctor,cleaning	,[so2,so3,so4]).

staff(d002,doctor,orthopaedist,[so2,so3,so4]).
staff(d003,doctor,orthopaedist,[so2,so3,so4]).
staff(d004,doctor,anesthesia     ,[so2,so3,so4]).

staff(e001, nurse,general	  ,[so2,so3,so4]).
staff(e002, nurse,general     ,[so2,so3,so4]).
staff(e003, nurse,general     ,[so2,so3,so4]).


agenda_operation_room(or1,20241028,[]).
agenda_operation_room(or2,20241028,[]).
agenda_operation_room(or3,20241028,[]).


% For the Complexity Study

agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
agenda_staff(d002,20241028,[(910,980,m02),(1380,1440,c02)]).
agenda_staff(d003,20241028,[(720,790,m01),(910,980,m02)]).
agenda_staff(d004,20241028,[(850,900,m02),(940,980,c04)]).


timetable(d001,20241028,(480,1200)).
timetable(d002,20241028,(500,1440)).
timetable(d003,20241028,(520,1320)).
timetable(d004,20241028,(480,1200)).

surgery_id(so100001,so2).
surgery_id(so100002,so3).
surgery_id(so100003,so4).
surgery_id(so100004,so2).
surgery_id(so100005,so4).
surgery_id(so100006,so2).
surgery_id(so100007,so3).
surgery_id(so100008,so2).
surgery_id(so100009,so2).
surgery_id(so100010,so2).
surgery_id(so100011,so4).
surgery_id(so100012,so2).
surgery_id(so100013,so2).

assignment_surgery(so100001,d001).
assignment_surgery(so100002,d002).
assignment_surgery(so100003,d003).
assignment_surgery(so100004,d001).
assignment_surgery(so100004,d002).
assignment_surgery(so100005,d002).
assignment_surgery(so100005,d003).
assignment_surgery(so100006,d001).
assignment_surgery(so100007,d003).
assignment_surgery(so100008,d004).
assignment_surgery(so100008,d003).
assignment_surgery(so100009,d002).
assignment_surgery(so100009,d004).
assignment_surgery(so100010,d003).
assignment_surgery(so100011,d001).
assignment_surgery(so100012,d001).
assignment_surgery(so100013,d004).


