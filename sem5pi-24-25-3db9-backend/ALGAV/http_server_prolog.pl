% https://github.com/Anniepoo/strangeloop
% https://www.swi-prolog.org/pldoc/man?section=httpserver

:-portray_text(true).

:-use_module(library(http/http_server)).
:-use_module('project_operation_room_scheduling3.pl').

:-initialization
   http_server([port(8080)]).
	
:-http_handler(root(.),
               http_redirect(moved, location_by_id(home_page)),
               []).
:-http_handler(root(home), home_page, []).

home_page(_Request) :-
	obtain_better_sol(or1, 20241028, RoomAgenda, LDoctorAgenda, Tfim),
    % cenvert to strings -> https://www.swi-prolog.org/pldoc/man?predicate=atom_string/2
    atom_string(TfimStr, Tfim),
    
    format_room_list(RoomAgenda, RoomAgendaStr),
    format_doctor_list(LDoctorAgenda, LDoctorAgendaStr),
    
    reply_html_page(
        title('Demo server'),
        [ h1('Hello world!'),
		p(['Room Agenda: ', RoomAgendaStr]),
        p(['Doctor Agenda List: ', LDoctorAgendaStr]),
        p(['End Time: ', Tfim])
        ]).
        
format_room_list([], '').
format_room_list([(Start, End, Room)|Tail], Result) :-
    format(string(Head), '(~w,~w,~w)', [Start, End, Room]),
    format_room_list(Tail, TailStr),
    ( TailStr = '' -> Result = Head
    ; string_concat(Head, ", ", TempResult),
      string_concat(TempResult, TailStr, Result) ).

% Format Doctor Agenda List as a string for HTML rendering
format_doctor_list([], '').
format_doctor_list([(Doctor, Agenda)|Tail], Result) :-
    format_doctor_agenda(Agenda, AgendaStr),
    format(string(Head), '( ~w: ~w)', [Doctor, AgendaStr]),
    format_doctor_list(Tail, TailStr),
    ( TailStr = '' -> Result = Head
    ; string_concat(Head, ", ", TempResult),
      string_concat(TempResult, TailStr, Result) ).


% Format a doctor's agenda as a string (a list of (Str, End, Surg) tuples)
format_doctor_agenda([], '').
format_doctor_agenda([(Str, End, Surg)|Tail], Result) :-
    format(string(Head), '(~w,~w,~w)', [Str, End, Surg]),
    format_doctor_agenda(Tail, TailStr),
    ( TailStr = '' -> Result = Head
    ; string_concat(Head, ", ", TempResult),
      string_concat(TempResult, TailStr, Result) ).
  
% dontpad.com/asist  
% https://www.scaler.com/topics/how-to-run-process-in-background-linux/
