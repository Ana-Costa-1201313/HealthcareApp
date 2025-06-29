%:- module(project_operation_room_scheduling3, [obtain_better_sol/5, brute_force/2, obtain_heuristic_solution/2]).

:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5.
:- dynamic counter/2.
:- dynamic room_summary/3.
:- dynamic room_free_agenda/3.
:- dynamic operation_assigned_to_room/3.

% Bibliotecas HTTP
:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_parameters)).
:- use_module(library(http/http_client)).
:- use_module(library(http/http_cors)).
% Bibliotecas JSON
:- use_module(library(http/json_convert)).
:- use_module(library(http/http_json)).
:- use_module(library(http/json)).

:- set_setting(http:cors, [*]).


% Cria��o de servidor HTTP no porto 'Port'
% http_stop_server(Port, _).					
server(Port) :-						
        http_server(http_dispatch, [port(Port)]).
		
		
:- http_handler('/atualizarBC'							, atualizarBC							, [method(post)]).
:- http_handler('/obtain_better_sol'					, run_obtain_better_sol				 	, [method(post)]).
:- http_handler('/brute_force'							, run_brute_force						, [method(get )]).
:- http_handler('/brute_force2'							, run_brute_force2						, [method(get )]).
:- http_handler('/obtain_heuristic_solution'			, run_obtain_heuristic_solution		 	, [method(get )]).
:- http_handler('/obtain_heuristic_occupied_solution'	, run_obtain_heuristic_occupied_solution, [method(get )]).
:- http_handler('/evaluate_average_ocupied_time'		, run_evaluate_average_ocupied_time	 	, [method(post)]).
:- http_handler('/evaluate_final_time'					, run_evaluate_final_time				, [method(post)]).
:- http_handler('/operations_to_rooms_automatic'		, run_operations_to_rooms_automatic		, [method(get)]).

atualizarBC(Request) :-
    cors_enable,
    format('Access-Control-Allow-Origin: ~w~n', [*]),
    format('Access-Control-Allow-Headers: ~w~n', [*]),
			http_read_data(Request, Data, [to(atom)]),
	(
		(
			save_file('bc.pl', Data),
			consult('bc.pl'),
			reply_json(['Base de conhecimento atualizada com sucesso!'], [json_object(dict)])
		);
		(	
			reply_json(['Atualizacao da Base de Conhecimento Falhou!'], [json_object(dict)])
		)
	).

save_file(FileName, Content) :-
    open(FileName, write, Stream),
    write(Stream, Content),
    close(Stream).


%	run_obtain_better_sol				 	 (Request):-
%		cors_enable,

run_brute_force(Request):-
    cors_enable,
    format('Access-Control-Allow-Origin: ~w~n', [*]),
    format('Access-Control-Allow-Headers: ~w~n', [*]),
	format('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS~n'),
    http_parameters(Request,
        [ room(Room_from_Request, []),
            day(Day_from_Request, []) 
        ]),
	term_to_atom(DayString,Day_from_Request),
	((once(brute_force(Room_from_Request,DayString)),!); true),
	agenda_operation_room1(Room_from_Request, DayString,AgendaR),
	to_string_list(AgendaR, AgendaRStr),
	findall(agenda_staff1(D, DayString, Agenda),agenda_staff1(D, DayString, Agenda),L),
	to_string_list(L,Lstr),
    Response = json{
        'AgendaRoom': AgendaRStr,
        'AgendasDoctor': Lstr
    },
    reply_json(Response).
	
run_brute_force2(Request):-
    cors_enable,
    format('Access-Control-Allow-Origin: ~w~n', [*]),
    format('Access-Control-Allow-Headers: ~w~n', [*]),
	format('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS~n'),
    http_parameters(Request,
        [ room(Room_from_Request, []),
            day(Day_from_Request, []) 
        ]),
	term_to_atom(DayString,Day_from_Request),
	((once(brute_force2(Room_from_Request,DayString)),!); true),
	agenda_operation_room1(Room_from_Request, DayString,AgendaR),
	to_string_list(AgendaR, AgendaRStr),
	findall(agenda_staff1(D, DayString, Agenda),agenda_staff1(D, DayString, Agenda),L),
	to_string_list(L,Lstr),
    Response = json{
        'AgendaRoom': AgendaRStr,
        'AgendasDoctor': Lstr
    },
    reply_json(Response).
	
	
	
to_string_list(ObjSwipl, StrList) :-
    maplist(term_string, ObjSwipl, StrList).
	

	
run_obtain_heuristic_solution(Request):-
	cors_enable,
    format('Access-Control-Allow-Origin: ~w~n', [*]),
    format('Access-Control-Allow-Headers: ~w~n', [*]),
    http_parameters(Request,
        [ room(Room_from_Request, []),
            day(Day_from_Request, []) 
        ]),
	term_to_atom(DayString,Day_from_Request),
	%((once(obtain_heuristic_solution(Room_from_Request,DayString)),!); true),
	obtain_heuristic_solution(Room_from_Request,DayString),
	agenda_operation_room1(Room_from_Request, DayString,AgendaR),
	to_string_list(AgendaR, AgendaRStr),
	findall(agenda_staff1(D, DayString, Agenda),agenda_staff1(D, DayString, Agenda),L),
	to_string_list(L,Lstr),
    Response = json{
        'AgendaRoom': AgendaRStr,
        'AgendasDoctor': Lstr
    },
    reply_json(Response).

run_obtain_heuristic_occupied_solution(Request):-
	cors_enable,
    format('Access-Control-Allow-Origin: ~w~n', [*]),
    format('Access-Control-Allow-Headers: ~w~n', [*]),
    http_parameters(Request,
        [ room(Room_from_Request, []),
            day(Day_from_Request, []) 
        ]),
	term_to_atom(DayString,Day_from_Request),
	((once(obtain_heuristic_occupied_solution(Room_from_Request,DayString)),!); true),
	%obtain_heuristic_occupied_solution(Room_from_Request,DayString),
	agenda_operation_room1(Room_from_Request, DayString,AgendaR),
	to_string_list(AgendaR, AgendaRStr),
	findall(agenda_staff1(D, DayString, Agenda),agenda_staff1(D, DayString, Agenda),L),
	to_string_list(L,Lstr),
    Response = json{
        'AgendaRoom': AgendaRStr,
        'AgendasDoctor': Lstr
    },
    reply_json(Response).
	
%	run_evaluate_average_ocupied_time	 	 (Request):-
%		cors_enable,
%	run_evaluate_final_time		             (Request):-
%		cors_enable,

run_operations_to_rooms_automatic(Request) :-
    cors_enable,
    format('Access-Control-Allow-Origin: *~n'),
    format('Access-Control-Allow-Headers: *~n'),
    format('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS~n'),

    % Extract day parameter from request
    http_parameters(Request, [ day(Day_from_Request, []) ]),

    % Convert day to atom
    term_to_atom(DayString, Day_from_Request),

    % Allocate surgeries for the given date
    allocate_surgeries(DayString),

    % Collect assigned surgeries for the date
    findall((Room, SurgeryId), operation_assigned_to_room(Room, DayString, SurgeryId), Assignments),
    to_string_list(Assignments, AssignmentList),

    % Return the response with the assigned surgeries
    Response = json{
        'AssignedOperations': AssignmentList
    },
    
    reply_json(Response).


%staff(d001	,doctor,orthopaedist,[so2,so3,so4]).
%staff(d0011	,doctor,anesthesia	,[so2,so3,so4]).
%staff(d0012 ,doctor,cleaning	,[so2,so3,so4]).
%
%staff(d002,doctor,orthopaedist,[so2,so3,so4]).
%staff(d003,doctor,orthopaedist,[so2,so3,so4]).
%staff(d004,doctor,general     ,[so2,so3,so4]).
%
%staff(e001, nurse,general	  ,[so2,so3,so4]).
%staff(e002, nurse,general     ,[so2,so3,so4]).
%staff(e003, nurse,general     ,[so2,so3,so4]).
%%
%%%surgery(SurgeryType,TAnesthesia,TSurgery,TCleaning).
%%
%%surgery(so2,45,60,45). % 150
%%surgery(so3,45,90,45). % 180	
%%surgery(so4,45,75,45). % 165
%%surgery(so4,45,20,20). % 85
%surgery(so2,1,60,1). % 150
%surgery(so3,1,90,1). % 180	
%surgery(so4,1,75,1). % 165
%surgery(so4,1,20,1). % 85
%surgery(so5,1,20,1). % 85
%
%agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
%agenda_operation_room(or2,20241028,[]).
%
%agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
%agenda_staff(d0011	,20241028,[]).
%agenda_staff(d0012	,20241028,[]).
%
%agenda_staff(d002,20241028,[]).
%agenda_staff(d003,20241028,[]).
%agenda_staff(d004,20241028,[]).
%%agenda_staff(d002,20241028,[(850,900,m02),(901,960,m02),(1380,1440,c02)]).
%%agenda_staff(d003,20241028,[(720,790,m01),(910,980,m02)]).
%%agenda_staff(d004,20241028,[(850,900,m02),(940,980,c04)]).
%
%timetable(d001,20241028,(480,1200)).
%timetable(d0011,20241028,(480,1200)).
%timetable(d0012,20241028,(480,1200)).
%
%timetable(d002,20241028,(500,1440)).
%timetable(d003,20241028,(520,1320)).
%timetable(d004,20241028,(620,1020)).
%
%surgery_id(so100001,so2).
%surgery_id(so100002,so3).
%surgery_id(so100003,so4).
%surgery_id(so100004,so2).
%% surgery_id(so100005,so4).
%% surgery_id(so100006,so2).
%% surgery_id(so100007,so3).
%% surgery_id(so100008,so2).
%% surgery_id(so100009,so2).
%% surgery_id(so100010,so5).
%% surgery_id(so100011,so4).
%% surgery_id(so100012,so2).
%% surgery_id(so100013,so2).
%
%assignment_surgery(so100001,d001).
%assignment_surgery(so100001,d0011).
%assignment_surgery(so100001,d0012).
%
%assignment_surgery(so100002,d002).
%assignment_surgery(so100003,d003).
%assignment_surgery(so100004,d001).
%assignment_surgery(so100004,d002).
%% assignment_surgery(so100005,d002).
%% assignment_surgery(so100005,d003).
%% assignment_surgery(so100006,d001).
%% assignment_surgery(so100007,d003).
%% assignment_surgery(so100008,d004).
%% assignment_surgery(so100008,d003).
%% assignment_surgery(so100009,d002).
%% assignment_surgery(so100009,d004).
%% assignment_surgery(so100010,d003).
%% assignment_surgery(so100011,d001).
%% assignment_surgery(so100012,d001).
%% assignment_surgery(so100013,d004).


free_agenda0([],[(0,1440)]).
free_agenda0([(0,Tfin,_)|LT],LT1):-!,free_agenda1([(0,Tfin,_)|LT],LT1).
free_agenda0([(Tin,Tfin,_)|LT],[(0,T1)|LT1]):- T1 is Tin-1,
    free_agenda1([(Tin,Tfin,_)|LT],LT1).

free_agenda1([(_,Tfin,_)],[(T1,1440)]):-Tfin\==1440,!,T1 is Tfin+1.
free_agenda1([(_,_,_)],[]).
free_agenda1([(_,T,_),(T1,Tfin2,_)|LT],LT1):-Tx is T+1,T1==Tx,!,
    free_agenda1([(T1,Tfin2,_)|LT],LT1).
free_agenda1([(_,Tfin1,_),(Tin2,Tfin2,_)|LT],[(T1,T2)|LT1]):-T1 is Tfin1+1,T2 is Tin2-1,
    free_agenda1([(Tin2,Tfin2,_)|LT],LT1).


adapt_timetable(D,Date,LFA,LFA2):-timetable(D,Date,(InTime,FinTime)),treatin(InTime,LFA,LFA1),treatfin(FinTime,LFA1,LFA2).

treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).

treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).


intersect_all_agendas([Name],Date,LA):-!,availability(Name,Date,LA).
intersect_all_agendas([Name|LNames],Date,LI):-
    availability(Name,Date,LA),
    intersect_all_agendas(LNames,Date,LI1),
    intersect_2_agendas(LA,LI1,LI).

intersect_2_agendas([],_,[]).
intersect_2_agendas([D|LD],LA,LIT):-	intersect_availability(D,LA,LI,LA1),
					intersect_2_agendas(LD,LA1,LID),
					append(LI,LID,LIT).

intersect_availability((_,_),[],[],[]).

intersect_availability((_,Fim),[(Ini1,Fim1)|LD],[],[(Ini1,Fim1)|LD]):-
		Fim<Ini1,!.

intersect_availability((Ini,Fim),[(_,Fim1)|LD],LI,LA):-
		Ini>Fim1,!,
		intersect_availability((Ini,Fim),LD,LI,LA).

intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)],[(Fim,Fim1)|LD]):-
		Fim1>Fim,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_).

intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)|LI],LA):-
		Fim>=Fim1,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_),
		intersect_availability((Fim1,Fim),LD,LI,LA).


min_max(I,I1,I,I1):- I<I1,!.
min_max(I,I1,I1,I).




schedule_all_surgeries(Room,Day):-
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Or,Date,Agenda),assert(agenda_operation_room1(Or,Date,Agenda)),
    findall(_,(agenda_staff1(D,Date,L),free_agenda0(L,LFA),adapt_timetable(D,Date,LFA,LFA2),assertz(availability(D,Date,LFA2))),_),
    findall(OpCode,surgery_id(OpCode,_),LOpCode),

    availability_all_surgeries(LOpCode,Room,Day),!.

availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),surgery(OpType,_,TSurgery,_),
    availability_operation(OpCode,Room,Day,LPossibilities,LDoctors),
    schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),
    availability_all_surgeries(LOpCode,Room,Day).



availability_operation(OpCode,Room,Day,LPossibilities,LDoctors):-surgery_id(OpCode,OpType),surgery(OpType,_,TSurgery,_),
    findall(Doctor,assignment_surgery(OpCode,Doctor),LDoctors),
    intersect_all_agendas(LDoctors,Day,LA),
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LFAgRoom),
    intersect_2_agendas(LA,LFAgRoom,LIntAgDoctorsRoom),
    remove_unf_intervals(TSurgery,LIntAgDoctorsRoom,LPossibilities).


remove_unf_intervals(_,[],[]).
remove_unf_intervals(TSurgery,[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-DT is Tfin-Tin+1,TSurgery=<DT,!,
    remove_unf_intervals(TSurgery,LA,LA1).
remove_unf_intervals(TSurgery,[_|LA],LA1):- remove_unf_intervals(TSurgery,LA,LA1).


schedule_first_interval(TSurgery,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery - 1.

insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors).



obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
		get_time(Ti),
		(obtain_better_sol1(Room,Day);true),
		retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
            %write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
            %write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
            %write('TFinOp='),write(TFinOp),nl,
		get_time(Tf),
		_T is (Tf-Ti).
		% write('Tempo de geracao da solucao:'),write(T),nl.


obtain_better_sol1(Room,Day):-
    asserta(better_sol(Day,Room,_,_,1441)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    permutation(LOC,LOpCode),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOpCode,Room,Day),
    agenda_operation_room1(Room,Day,AgendaR),
		update_better_sol(Day,Room,AgendaR,LOpCode),
		fail.

update_better_sol(Day,Room,Agenda,LOpCode):-
				retract(counter(Nome,Count)),
				CountNovo is Count +1,
				asserta(counter(Nome, CountNovo)),
                better_sol(Day,Room,_,_,FinTime),
                reverse(Agenda,AgendaR),
                evaluate_final_time(AgendaR,LOpCode,FinTime1),
             %write('Analysing for LOpCode='),write(LOpCode),nl,
             %write('now: FinTime1='),write(FinTime1),write(' Agenda='),write(Agenda),nl,
		FinTime1<FinTime,
             %write('best solution updated'),nl,
                retract(better_sol(_,_,_,_,_)),
                findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
                remove_equals(LDoctors1,LDoctors),
                list_doctors_agenda(Day,LDoctors,LDAgendas),
		asserta(better_sol(Day,Room,Agenda,LDAgendas,FinTime1)).
		
update_better_sol_occupied_average(Day,Room,Agenda):-
				retract(counter(Nome,Count)),
				CountNovo is Count +1,
				asserta(counter(Nome, CountNovo)),
                better_sol(Day,Room,_,_,AVG_Best_Current),
				evaluate_average_ocupied_time(Room, Day, AVG),
		AVG < AVG_Best_Current,
				%write('old sol:'),write(better_sol(Day,Room,_,_,_AAA)),nl,
                retract(better_sol(_,_,_,_,_)),
                findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
                remove_equals(LDoctors1,LDoctors),
                list_doctors_agenda(Day,LDoctors,LDAgendas),
		%asserta(better_sol(Day,Room,Agenda,LDAgendas,AVG)),
		%write('new sol:'),write(better_sol(Day,Room,Agenda,LDAgendas,AVG)),nl.
		asserta(better_sol(Day,Room,Agenda,LDAgendas,AVG)).


evaluate_final_time([],_,1441).
evaluate_final_time([(_,Tfin,OpCode)|_],LOpCode,Tfin):-member(OpCode,LOpCode),!.
evaluate_final_time([_|AgR],LOpCode,Tfin):-evaluate_final_time(AgR,LOpCode,Tfin).



list_doctors_agenda(_,[],[]).
list_doctors_agenda(Day,[D|LD],[(D,AgD)|LAgD]):-agenda_staff1(D,Day,AgD),list_doctors_agenda(Day,LD,LAgD).

remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

encontrar_todos_os_pares(LFINAL):-
	findall(OpCode,surgery_id(OpCode, _), L),
	findall(Room,agenda_operation_room(Room, _,_), LR),
	findall((OpCode, Room),(member(OpCode, L), member(Room, LR)), LFINAL).

encontrar_todas_permut_cirurgia_unica(L, FINAL):-
	encontrar_permutacao(L, [], FINAL).

encontrar_permutacao([], ACC, ACC). % write('		cheguei ao fim da lista de operacoes'), nl.
encontrar_permutacao([(OpCode, Room)|T], ACC, Result):-
	\+member((OpCode,_), ACC),
	encontrar_permutacao(T, [(OpCode, Room)|ACC], Result).
encontrar_permutacao([(_, _)|T], ACC, Result):-
	%nl,write('		nao unifiquei'), nl,
	encontrar_permutacao(T, ACC, Result).
	


limpar_agendas():-
	retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
	% END : remover da memoria os agendamentos que j� existem
	% START : para todos os agendamentos de doutores/cirurgias determinados adicionar � lista de c�lculos 
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    findall(_,(agenda_operation_room(Or,Date,Agenda),assert(agenda_operation_room1(Or,Date,Agenda))),_),
    findall(_,(agenda_staff1(D,Date,L),free_agenda0(L,LFA),adapt_timetable(D,Date,LFA,LFA2),assertz(availability(D,Date,LFA2))),_), !.
	% END : para todos os agendamentos de doutores/cirurgias determinados adicionar � lista de c�lculos


get_staff_with_specialty(Specialty, LDoctor, LNurse):-
	%findall(staff(ID,'doctor',Specialty,Duties),staff(ID,'doctor', Specialty,Duties), LDoctor),
	findall(ID,staff(ID,'doctor', Specialty,Duties), LDoctor),
	%findall(staff(ID,'nurse',Specialty,Duties),staff(ID,'nurse', Specialty,Duties), LNurse).
	findall(ID,staff(ID,'nurse', Specialty,Duties), LNurse).
	
get_coordinated_scheduled_staff(Specialty,DAte,LStaff, LI):-
	get_staff_with_specialty(Specialty, LDoctor, LNurse),
	
	%ordenar LDoctor pelo horario de disponibilidade
	%ordenar LNurse  pelo horario de disponibilidade
	
	append(LDoctor, LNurse, LStaff),
	%(
	%intersect_all_agendas(LStaff, DAte, LI);
	%intersect_all_agendas(LDoctor, DAte, LI)
	%).
	%find_free_agendas(DAte),
	intersect_all_agendas(LStaff, DAte, LI).
	

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% findall(agenda_staff(D,Day,Agenda),agenda_staff(D,Day,Agenda), R).
% findall(agenda_operation_room1(OR, Day, AgendaOR), agenda_operation_room1(OR, Day, AgendaOR), LOpRoom), findall(agenda_staff1(Staff, Day, AgendaStaff), agenda_staff1(Staff, Day, AgendaStaff), LStaff).
% limpar_agendas
% set_prolog_flag(answer_write_options,[max_depth(0)]).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%ALGAV 5.3.1%%%%%%%%%%%%%%%%%%%%%%%%%%%%


brute_force(Room, Day):-
		retractall(counter(_,_)),
		assertz(counter('bruto', 0)),
		get_time(Ti),
		(obtain_better_sol2(Room,Day);true),
		retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
            write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
            write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
            write('TFinOp='),write(TFinOp),nl,
			write('Number of valid solutions='),counter(_,C), write(C), nl,
		get_time(Tf),
		T is (Tf-Ti),
		write('Tempo de geracao da solucao:'),write(T),nl.
	
brute_force2(Room, Day):-
	retractall(counter(_,_)),
	asserta(counter('bruto2', 0)),
	get_time(Ti),
	(obtain_better_sol2_2(Room,Day);true),
	retract(better_sol(Day,Room,_AgOpRoomBetter,_LAgDoctorsBetter,_TFinOp)),
        % write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
        % write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
        % write('TFinOp='),write(TFinOp),nl,
		% write('Number of valid solutions='),counter(_,C), write(C), nl,
	get_time(Tf),
	_T is (Tf-Ti).
	% write('Tempo de geracao da solucao:'),write(T),nl.
		
obtain_better_sol2(Room,Day):-
    asserta(better_sol(Day,Room,_,_,1441)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    permutation(LOC,LOpCode),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries1(LOpCode,Room,Day),						%aqui - check
    agenda_operation_room1(Room,Day,AgendaR),
		update_better_sol(Day,Room,AgendaR,LOpCode),  					%aqui
		fail.
		
obtain_better_sol2_2(Room,Day):-
    asserta(better_sol(Day,Room,_,_,1441)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    permutation(LOC,LOpCode),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries1(LOpCode,Room,Day),						%aqui - check
    agenda_operation_room1(Room,Day,AgendaR),
		update_better_sol_occupied_average(Day,Room,AgendaR),  					%aqui
		fail.
	
	
availability_all_surgeries1([],_,_).
availability_all_surgeries1([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),surgery(OpType,TAnesthesia,TSurgery,TCleaning),
	TTotal is TAnesthesia+TSurgery+TCleaning,								% somatorio -> tempo total de procedimento
    availability_operation1(OpCode,Room,Day,LPossibilities,_LDoctors),	%aqui - check
    schedule_first_interval(TTotal,LPossibilities,(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),					%aqui - 
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    % insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),			%aqui - insert agenda doctors for each part of the surgery
	
	get_staff_with_specialty_new(OpCode, anesthesia,LAnesthesia),
    _TinAnesthesia is TinS,
    TfinAnesthesia is TinS + TAnesthesia+TSurgery, 
	insert_agenda_doctors((TinS,TfinAnesthesia,OpCode),Day,LAnesthesia),
	
	get_staff_with_specialty_new(OpCode, orthopaedist,LSurgery),
    TinSurgery is TinS + TAnesthesia,
    TfinSurgery is TinSurgery + TSurgery,   
	insert_agenda_doctors((TinSurgery,TfinSurgery,OpCode),Day,LSurgery),
	
	get_staff_with_specialty_new(OpCode, cleaning,LCleaning),
    TinCleaning is TfinSurgery,
    _TfinCleaning is TfinS,    
	insert_agenda_doctors((TinCleaning,TfinS,OpCode),Day,LCleaning),!,
    
	availability_all_surgeries1(LOpCode,Room,Day),!.
	
	
availability_operation1(OpCode,Room,Day,LPossibilities,LDoctors):-
	surgery_id(OpCode,OpType),surgery(OpType,TAnesthesia,TSurgery,TCleaning),
    findall(Doctor,assignment_surgery(OpCode,Doctor),LDoctors),
    intersect_all_agendas(LDoctors,Day,LA),
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LFAgRoom),
    intersect_2_agendas(LA,LFAgRoom,LIntAgDoctorsRoom),
	TTotal is TAnesthesia+TSurgery+TCleaning,
    remove_unf_intervals(TTotal,LIntAgDoctorsRoom,LPossibilities).
	

get_staff_with_specialty_new(OpCode, Specialty, LStaff):-
	% findall(ID,(staff(ID,_,anesthesia,Duties), assignment_surgery(s100002,ID)), LStaff),
	findall(ID,(staff(ID,_,Specialty,_), assignment_surgery(OpCode,ID)), LStaff).



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%ALGAV 5.3.3.1%%%%%%%%%%%%%%%%%%%%%%%%%%%%

obtain_heuristic_solution(Room, Day):-
	findall(OpCode,surgery_id(OpCode,_),LOC),
	retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
	
	get_time(Ti),
	(heuristic(LOC, Day,Room);true),
	agenda_operation_room1(Room,Day, AgendaRoom),
	findall((D, AgendaDoctor)  ,(agenda_staff1(D, Day, AgendaDoctor))		  , _LAgDoctorsBetter),
	reverse(AgendaRoom, [ (_,_TFinOp,_)	| _RESTOS]),
        % write('Final Result for '), write(Room),write(': AgOpRoomBetter='),write(AgendaRoom),nl,
        % write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
        % write('TFinOp='),write(TFinOp),nl,
		evaluate_average_ocupied_time(Room, Day,_),
		evaluate_final_time(Room, Day),
	get_time(Tf),
	_T is (Tf-Ti).
	
	
heuristic([],_,_).
heuristic(LOpCodes,Day,Room):- 
%	1. Calcular opera�ao que pode come�ar mais cedo
	heuristicaEarly(LOpCodes,Day,Room,EarliestOpCode,EarliestTime),
%	2. Marcar Agendas Medico / Room
	marcar_operacao(EarliestOpCode,Day,Room,EarliestTime),
	delete(LOpCodes, EarliestOpCode, LOpCodes2),!,
	heuristic(LOpCodes2,Day,Room),!.
	

heuristicaEarly([OpCode],Day,Room, OpCode, (Str,End)):-
	availability_operation1(OpCode,Room,Day,[(Str,End)|_],_LStaff).
	
heuristicaEarly([OpCode|LOpCodes],Day, Room, EarliestOpCode, (S,F)):-
	heuristicaEarly(LOpCodes,Day, Room, OpCodeComp, (Str,End)),
%	1.buscar a cirurgia com o horario compativel mais cedo
	availability_operation1(OpCode,Room,Day,[(StrActual,EndActual)|_],_LStaff),
%	2.comparar tempos de come�o
	(
		(StrActual<Str, EarliestOpCode=OpCode,S=StrActual,F=EndActual,!);
		(EarliestOpCode=OpCodeComp,S=Str,F=End)
	).
	

marcar_operacao(OpCode,Day,Room,(Str,Fin)):-
	surgery_id(OpCode,OpType),surgery(OpType,TAnesthesia,TSurgery,TCleaning),
%	1.buscar tempo total opera��o
	TTotal is TAnesthesia+TSurgery+TCleaning,								% somatorio -> tempo total do procedimento
%	2.buscar bloco de tempo do tamanho de TTotal    
	schedule_first_interval(TTotal,[(Str,Fin)],(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),					%aqui - 
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    % insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),			%aqui - insert agenda doctors for each part of the surgery
	
%	3.marcar os medicos correspondentes �s diferentes partes da opera�ao
	get_staff_with_specialty_new(OpCode, anesthesia,LAnesthesia),
    _TinAnesthesia is TinS,
    TfinAnesthesia is TinS + TAnesthesia+TSurgery, 
	insert_agenda_doctors((TinS,TfinAnesthesia,OpCode),Day,LAnesthesia),
	
	get_staff_with_specialty_new(OpCode, orthopaedist,LSurgery),
    TinSurgery is TinS+TAnesthesia,
    TfinSurgery is TinSurgery + TSurgery,   
	insert_agenda_doctors((TinSurgery,TfinSurgery,OpCode),Day,LSurgery),
	
	get_staff_with_specialty_new(OpCode, cleaning,LCleaning),
    TinCleaning is TfinSurgery,
    _TfinCleaning is TfinS,    
	insert_agenda_doctors((TinCleaning,TfinS,OpCode),Day,LCleaning).
	
	
	
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%ALGAV 5.3.3.2%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%doctor_occupied_time(NAME, Percentage_Time).
:- dynamic doctor_occupied_time/2.

obtain_heuristic_occupied_solution(Room, Day):-
	findall(OpCode,surgery_id(OpCode,_),_LOC),
	retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
	findall(OpCode, surgery_id(OpCode,_), LOpCode),
	heuristicaOcupied(LOpCode,[], Room, Day),
	agenda_operation_room1(Room,Day,_AgendaRoom),!,
	findall(agenda_staff1(D,Day,AG), agenda_staff1(D,Day,AG), _LAgendaDoctor),
		% write('Final Result for '), write(Room),write(': AgendaRoom='),write(AgendaRoom),nl,
        % write('LAgendaDoctor='),write(LAgendaDoctor),nl,
		evaluate_average_ocupied_time(Room, Day,_AVG),
		evaluate_final_time(Room, Day),
		!.


%findall(OpCode, surgery_id(OpCode,_), L),heuristicaOcupied(L, [], or1, 20241028).
heuristicaOcupied([],_,_,_).
heuristicaOcupied(LOpCode,Visited,Room,Day):-
	retractall(doctor_occupied_time(_,_)),
	findall(
			Doctor,
			(
				surgery_id(Sur,_),
				assignment_surgery(Sur, Doctor)
			),
			LD
		),
	sort(LD, LDoctor),
	doctor_occupied_time_calc(LDoctor, Day),nl,
%2.ordenar a lista por Percentage_Time
	findall((Doc, P),doctor_occupied_time(Doc, P), LDP),
	sort(LDP, Temp),
	%predsort(compare_percentage, Temp, SortedLDP),
	%sort(SortedLDP,L1),
%3.selecionar o primeiro elemento
	%select_first(L1, D),
	%select_first(LDP, D),
	%select_first(SortedLDP, D),
	select_most_ocupied_doctor(Temp, (D,_)),
	findall(OpCode, assignment_surgery(OpCode,D),LC),
%4.marcar cirurgia
	select_op_code_not_visited(LC, Visited, OpCode, _RestOpCodes),!,
	((availability_all_surgeries2(OpCode,Room,Day), !);true),!,
	delete(LOpCode, OpCode, LOpCodes2),
	heuristicaOcupied(LOpCodes2,[OpCode | Visited],Room, Day).
	

select_first([(D,_)|_T], D).

select_most_ocupied_doctor([(Doctor, Ocupacao)], (Doctor, Ocupacao)).
select_most_ocupied_doctor([(D, O)|Lista_ocupacoes], (Doctor, Ocupacao)):-
	select_most_ocupied_doctor(Lista_ocupacoes, (Doctor1, Ocupacao1)),
	(
		(O > Ocupacao1, Doctor = D, Ocupacao is O, !);
		(Doctor = Doctor1, Ocupacao is Ocupacao1)
	).
	

select_op_code_not_visited([OpCode | RestOpCodes], Visited, OpCode, RestOpCodes) :-
    \+ member(OpCode, Visited).  % If OpCode is not in Visited, select it.
select_op_code_not_visited([_ | RestOpCodes], Visited, OpCode, RemainingOpCodes) :-
    select_op_code_not_visited(RestOpCodes, Visited, OpCode, RemainingOpCodes).	

doctor_occupied_time_calc([],_).
doctor_occupied_time_calc([Doctor | LDoctor], Day):-

	retractall(doctor_occupied_time(_,_,_)),
	
	timetable(Doctor,Day,(TimetableStr,TimetableEnd)),
	SizeHorario=TimetableEnd-TimetableStr,
	staff(Doctor,_Type,Specialty,_),
	findall(
		(
			TAnesthesia,
			TSurgery,
			TCleaning
		),
		(
			assignment_surgery(SG,Doctor), 
			surgery_id(SG,TP), 
			surgery(TP,TAnesthesia,TSurgery,TCleaning),
			agenda_staff1(Doctor, Day, LAgenda),
			\+ member((_, _, SG), LAgenda)
		),
		LOps
	),
	(
		(
			LOps \= [],!,
			sum_time_ops(LOps, Specialty, TimeOccupied),
			
			agenda_staff1(Doctor,Day,LAgenda),
			total_occupied_scheduled_time(LAgenda, TotalScheduledTime),
			
			Percentage_Time is (TotalScheduledTime+TimeOccupied)/SizeHorario,
			
			assertz(doctor_occupied_time(Doctor, Percentage_Time)),!
		);
		(
			assertz(doctor_occupied_time(Doctor, 0)),!
		)
	),
	% nl,write(doctor_occupied_time(Doctor, Percentage_Time)),
	doctor_occupied_time_calc(LDoctor, Day).
	

not_in_agenda(SG, LAgenda) :-
    \+ member((_, _, SG), LAgenda).	

total_occupied_scheduled_time([], 0).
total_occupied_scheduled_time([(Start, Finish, _Op) | Rest], TotalTime) :-
    IntervalTime is Finish - Start,
    total_occupied_scheduled_time(Rest, RestTime),
    TotalTime is IntervalTime + RestTime.

sum_time_ops([], _, 0).
%Caso especialidade orthopaedist
sum_time_ops([(_TAnesthesia,TSurgery,_TCleaning) | LOps], orthopaedist, TimeOccupied):-
	sum_time_ops(LOps, orthopaedist, TimeOccupied1),
	TimeOccupied is TSurgery+TimeOccupied1.
	
%Caso especialidade anesthesia
sum_time_ops([(TAnesthesia,TSurgery,_TCleaning) | LOps], anesthesia, TimeOccupied):-
	sum_time_ops(LOps, anesthesia, TimeOccupied1),
	TimeOccupied is TAnesthesia+TSurgery+TimeOccupied1.

%Caso especialidade cleaning
sum_time_ops([(_TAnesthesia,_TSurgery,TCleaning) | LOps], cleaning, TimeOccupied):-
	sum_time_ops(LOps, cleaning, TimeOccupied1),
	TimeOccupied is TCleaning+TimeOccupied1.
	
% Caso Geral
sum_time_ops([(TAnesthesia, TSurgery, TCleaning) | LOps], _, TimeOccupied) :-
    sum_time_ops(LOps, other, TimeOccupied1),
    TimeOccupied is TAnesthesia + TSurgery + TCleaning + TimeOccupied1.

% comparador
compare_percentage(<, (_, P1), (_, P2)):-
    P1 < P2.                   
compare_percentage(>, (_, P1), (_, P2)):-
    P1 > P2.                   
compare_percentage(=, (_, P1), (_, P2)):-
    P1 =:= P2.

%findall((Doctor, P),doctor_occupied_time(Doctor, P), LDP),predsort(compare_percentage/3, LDP, SortedLDP).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

availability_all_surgeries2(OpCode,Room,Day):-
    surgery_id(OpCode,OpType),surgery(OpType,TAnesthesia,TSurgery,TCleaning),
	TTotal is TAnesthesia+TSurgery+TCleaning,								% somatorio -> tempo total de procedimento
    availability_operation1(OpCode,Room,Day,LPossibilities,_LDoctors),	%aqui - check
    schedule_first_interval(TTotal,LPossibilities,(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),					%aqui - 
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    % insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),			%aqui - insert agenda doctors for each part of the surgery
	
	get_staff_with_specialty_new(OpCode, anesthesia,LAnesthesia),
    _TinAnesthesia is TinS,
    TfinAnesthesia is TinS + TAnesthesia + TSurgery, 
	insert_agenda_doctors((TinS,TfinAnesthesia,OpCode),Day,LAnesthesia),
	
	get_staff_with_specialty_new(OpCode, orthopaedist,LSurgery),
    TinSurgery is TinS + TAnesthesia,
    TfinSurgery is TinSurgery + TSurgery,   
	insert_agenda_doctors((TinSurgery,TfinSurgery,OpCode),Day,LSurgery),
	
	get_staff_with_specialty_new(OpCode, cleaning,LCleaning),
    TinCleaning is TfinSurgery,
    TfinCleaning is TinCleaning+TCleaning,    
	insert_agenda_doctors((TinCleaning,TfinCleaning,OpCode),Day,LCleaning).
	
	
	
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Evaluation %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

evaluate_average_ocupied_time(_Room, Day, AVG):-
	findall((Doctor,Day),agenda_staff1(Doctor,Day, _Agenda), LAgendaDoctor),
	all_doctor_ocupied_time(LAgendaDoctor, SUM, NUM),
	AVG is SUM/NUM.
	%AVG is SUM/NUM,
	%write('The average ocupancy time is: '),write(AVG),nl.

all_doctor_ocupied_time([(Doctor, Day)] , SUM, 1):-
	this_doctor_ocupied_time(Doctor, Day, SUM).
all_doctor_ocupied_time([(Doctor, Day) | Tail] , SUM, NUM):-
	all_doctor_ocupied_time(Tail , SUM1, NUM1),
	this_doctor_ocupied_time(Doctor, Day, SumOccupiedTime),
	SUM is SUM1 + SumOccupiedTime,
	NUM is NUM1 + 1.
	

this_doctor_ocupied_time(Doctor, Day, SumOccupiedTime):-
	agenda_staff1(Doctor, Day, Agenda),
	sum_occupied_time(Agenda, SumOccupiedTime).
	
sum_occupied_time([], 0).
sum_occupied_time([(Str, Fin, _Surg) | Tail], SUM):-
	sum_occupied_time(Tail, SUM1),
	SUM is SUM1 + (Fin-Str).


evaluate_final_time(Room, Day):-
	agenda_operation_room1(Room, Day, Agenda),
	reverse(Agenda, [ (_Str,_Fin,_Surg) |_Tail]).
	%reverse(Agenda, [ (Str,Fin,_Surg) |Tail]),
	%write('The last prceadure ends at: '),write(Fin),nl.
	

		
		

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%% THIS IS MOSTLY TESTING FOR 7.3.1 %%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%room_summary(Room, TotalAvailableTime, SumOfOperationsTime).
%:- dynamic room_summary/3.

%room_free_agenda(Room, Date, FreeSlots).
%:- dynamic room_free_agenda/3.

%operation_assigned_to_room(Room, Date, OperationId).
%:- dynamic operation_assigned_to_room/3.



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%this gives the availability slots of room(or1)
%agenda_operation_room(or1, 20241028, Agenda), free_agenda0(Agenda, FreeSlots).




%this gives the availability slots of all rooms
%run with --> free_agendas_for_all_rooms(20241028, FreeSlots).
 
free_agendas_for_all_rooms(Date, FreeAgendas) :-					
    findall((Room, FreeSlots), 										% Find all rooms with their agendas
	(agenda_operation_room(Room, Date, Agenda),
        free_agenda0(Agenda, FreeSlots),		
        (retractall(room_free_agenda(Room, Date, _)), 				% Remove old entry if it exists
         assertz(room_free_agenda(Room, Date, FreeSlots)))			% Save the computed FreeSlots in room_free_agenda/3
    ), FreeAgendas).

	
	
	
%%this calculates the total time occupied by the operations already scheduled to that room
%% --> calculate_sum_from_agenda(Room, 20241028, Total).

% Calculate the total sum of operations already scheduled in a room
calculate_sum_from_agenda(Room, Date, SumOfOperations) :-
    agenda_operation_room(Room, Date, Agenda),						% Get the agenda of assigned operations for the room
    sum_agenda_slots(Agenda, SumOfOperations).						% Calculate total occupied time from assigned operations


% Sum the durations of time slots in the agenda
sum_agenda_slots([], 0).
sum_agenda_slots([(Start, End, _) | Rest], TotalTime) :-
    SlotTime is End - Start,
    sum_agenda_slots(Rest, RestTotal),
    TotalTime is SlotTime + RestTotal.

initialize_room_summary(Date) :-
    retractall(room_summary(_, _, _)),								    % Remove existing room_summary/3 facts for the given date
    findall(Room, room_free_agenda(Room, Date, _), Rooms),				% Find all rooms with free agenda on the given date
    forall(member(Room, Rooms), 										% Initialize room summaries
	(calculate_sum_from_agenda(Room, Date, SumOfOperations),
        assertz(room_summary(Room, 1440, SumOfOperations))
    )).



%this calculates the amount of time a surgery takes
%run with --> surgery_total_time(Type,Total).
%surgery_type(so3,45,75,45). --> 165

%Calculate total time for a surgery type
surgery_total_time(Type, TotalTime) :-
    surgery_type(Type, Prep, Duration, Recovery),
    TotalTime is Prep + Duration + Recovery.

% Calculate all surgeries with their total times
surgery_with_time(Id, TotalTime) :-
    surgery_id(Id, Type),
    surgery_total_time(Type, TotalTime).

% Collect and sort(DESC) surgeries by total time
sort_surgeries_by_time(SortedSurgeries) :-
    findall((Id, TotalTime), surgery_with_time(Id, TotalTime), SurgeryList),
    sort(2, @>=, SurgeryList, SortedSurgeries).







% Find the best room and allocate the surgery
allocate_surgery_to_best_room(SurgeryId, SurgeryTime, Date) :-
																					% Step 3: Get all room summaries
    findall((Room, TotalTime, SumOfOp), room_summary(Room, TotalTime, SumOfOp), RoomSummaries),

																					% Step 4: Calculate ratios and filter rooms
    findall((Room, NewRatio), (
        member((Room, TotalTime, SumOfOp), RoomSummaries),
        NewSum is SumOfOp + SurgeryTime,
        NewRatio is NewSum / TotalTime,
        NewRatio =< 0.8  															% Ensure the ratio is lower or equal to 0.8
    ), ValidRooms),

																					% Step 5: Sort(ASC) and select room with the lowest ratio
    sort(2, @=<, ValidRooms, [ (BestRoom, _) | _ ]),

																					% Step 6: Allocate surgery and update room summary
    update_room_summary(BestRoom, SurgeryTime, SurgeryId, Date).

% Update the room summary with new surgery and track assignment
update_room_summary(Room, SurgeryTime, SurgeryId, Date) :-
																					% Update total time used in the room
    retract(room_summary(Room, TotalTime, SumOfOp)),
    NewSum is SumOfOp + SurgeryTime,
    assertz(room_summary(Room, TotalTime, NewSum)),

																					% Track the assigned surgery with room and date
    assertz(operation_assigned_to_room(Room, Date, SurgeryId)).                    	%%PONTO FINAL / VIRGULA para prints

%    format('Allocated Surgery ~w to Room ~w on ~w~n', [SurgeryId, Room, Date]).




%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%% THIS IS THE ONLY THING THAT NEEDS TO BE RUN FOR 7.3.1 %%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Allocate surgeries to rooms
allocate_surgeries(Date) :-
																					% Step 0: Update room agendas and summaries
    free_agendas_for_all_rooms(Date, _),   											% Recalculate free agendas for all rooms
    initialize_room_summary(Date),         											% Recalculate room summaries (available time and used time)
    
																					% Clear previous assignments for the date
    retractall(operation_assigned_to_room(_, Date, _)),

																					% Step 1: Sort surgeries by descending total time
    sort_surgeries_by_time(SortedSurgeries),

																					% Step 2: Process each surgery one by one
    forall(member((SurgeryId, SurgeryTime), SortedSurgeries), (
        allocate_surgery_to_best_room(SurgeryId, SurgeryTime, Date)
    )).



%%para ver o tempo ocupado de cada sala
% --> room_summary(Room,TotalTime, SumOfOp).

%%para ver em que sala cada operaçao ficou
% --> operation_assigned_to_room(Room,20241028,SurgeryId).
		
