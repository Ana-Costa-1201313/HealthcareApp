%:- module(geneticAlgorithmAppointment, []).

:-dynamic day/1.
:-dynamic room/1.
:-dynamic tarefas/1.
:-dynamic tarefas_room/6.
:-dynamic latest_gen_indivuduals/1.
:-dynamic resultPerRoom/3. %(Room, agenda_operation_room1, L_agenda_staff1)
:-dynamic agenda_operation_room2/3.
:-dynamic agenda_staff2/3.

% tarefas_room(Day,Room, OpCode ,T_processamento, T_Conc, Penalizacao)

elite(10).	
% turn on trace(retira/4).


%	full(Day):-
%		% retract de variaveis utilizadas 
%		retractall(resultPerRoom(_,_,_)),
%		retractall(operation_assigned_to_room(_,_,_)),
%		retractall(agenda_operation_room2(_,_,_)),
%		retractall(agenda_staff2(_,_,_)),
%		% allocar cirurgias a salas
%		allocate_surgeries(Day),
%		% criar lista das salas existentes
%		findall(Room, agenda_operation_room(Room,B,C), LRoom),
%		% criar as agendas novas qe vao ser preenchidas
%		findall(_, (agenda_staff(AS1,AS2,AS3), assertz(agenda_staff2(AS1,AS2,AS3))),_),
%		findall(_, (agenda_operation_room(AR1,AR2,AR3), assertz(agenda_operation_room2(AR1,AR2,AR3))),_),
%		%((length(LRoom, LLRoom), LLRoom == 1, whatever);start(LRoom, Day, Result)),
%		% brute_force abaixo de 6 cirurgias
%		% heuritica acima de e incluindo 6 cirurgias. 
%		% começar o agendamento com recurso a algoritmos geneticos
%		start(LRoom, Day, Result),
%		findall(resultPerRoom(Room, A, LAG), resultPerRoom(Room, A, LAG), L),
%		nl,nl,nl,write('Final: '), write(L), nl.
%		
full(Day):-
	% retract de variaveis utilizadas 
	retractall(resultPerRoom(_,_,_)),
	retractall(operation_assigned_to_room(_,_,_)),
	retractall(agenda_operation_room2(_,_,_)),
	retractall(agenda_staff2(_,_,_)),
	% allocar cirurgias a salas
	allocate_surgeries(Day),
	% criar lista das salas existentes
	findall(Room, agenda_operation_room(Room,B,C), LRoom),
	% criar as agendas novas qe vao ser preenchidas
	findall(_, (agenda_staff(AS1,AS2,AS3), assertz(agenda_staff2(AS1,AS2,AS3))),_),
	findall(_, (agenda_operation_room(AR1,AR2,AR3), assertz(agenda_operation_room2(AR1,AR2,AR3))),_),
	% bloco de condioes para decidir que tipo de algoritmo usar para o agendamento
	% se o numero de salas for 1 -> brute_force ou heuristica
	% se o numero de cirurgias for inferior ou igual a 6 -> brute_force, caso contrario heuristica com base em tempo final.
	length(LRoom, LLRoom),
	(
		(
			LLRoom = 1,!,
			(
				LRoom = [Room1],
				findall(sID, surgery_id(sID, _), LsID),
				length(LsID, NLsID),
				(
					NLsID =< 6,
					brute_force(Room1, Day),!
				);
				(
					obtain_heuristic_solution(Room1, Day)
				)
			)
		)
		;
		start(LRoom, Day, Result)
	),
	findall(resultPerRoom(Room, A, LAG), resultPerRoom(Room, A, LAG), L).
	%nl,nl,nl,write('Final: '), write(L), nl.
	
	


start([],_,[]).	
start([Room|Rest], Day, [Result|Res]):-
	% Recursivamente agendar as cirurgias para cada sala
	comeca_aqui(Room, Day),
	findall(agenda_staff1(D, Day, AG), agenda_staff1(D, Day, AG), LAG),
	agenda_operation_room1(Room, Day, A),
	assert(resultPerRoom(Room, A, LAG)),
	% atualizar as agendas das Room
	retract(agenda_operation_room2(Room,_,_)),
	retract(agenda_operation_room1(Room,AR2,AR3)),
	assertz(agenda_operation_room2(Room,AR2,AR3)),
	% atualizar as agendas de Straff
	retractall(agenda_staff2(_,_,_)),
	findall(_, (agenda_staff1(AS1,AS2,AS3), assertz(agenda_staff2(AS1,AS2,AS3))),_),
	retractall(tarefas(_)),
	start(Rest, Day, Res).

	


comeca_aqui(Room,Day):-
	retractall(day(_)),
	retractall(room(_)),
	retractall(geracoes(_)),
	retractall(populacao(_)),
	retractall(prob_cruzamento(_)),
	retractall(prob_mutacao(_)),!,
	asserta(day(Day)),
	asserta(room(Room)),
	asserta(geracoes(5)),
	asserta(populacao(4)),
	asserta(prob_cruzamento(0.70)),
	asserta(prob_mutacao(0.01)),!,
	% gerar populaçao inicial
	gera_populacao_schedule(Pop),					
	length(Pop, NPop),
	%write('Day='),write(Day),nl,																																
	%write('Pop='),write(NPop),write(':'),write(Pop),nl,		!,	
	% avaliar os individos de uma dada populacao
	avalia_populacao_schedule(Room,Day,Pop,PopAv),																																		
	%write('PopAv='),write(PopAv),nl,
	% ordenar os individuos com base no Fitness
	ordena_populacao(PopAv,PopOrd),																																		
	geracoes(NG),			!,
	% correr o algoritmo genetico, com NG geracoes
	gera_geracao_schedule(Room,Day,0,NG,PopOrd),!,
	% produto da geraçao final
	latest_gen_indivuduals(PPP),
	% escolher o melhor individo
	reverse(PPP, [P*V|R]),
	%write('Popfinal='), write(P*V), nl,
	% agendar o melhor individo
	(availability_all_surgeries1(Room, Day, P); true),
	agenda_operation_room1(Room, Day, A),
	%write('agenda_operation_room1='), write(A), nl,
	findall(agenda_staff1(D, Day, AG), agenda_staff1(D, Day, AG), LAG).
	%write('L_agenda_staff1='), write(LAG), nl.
	% write('Popfinal='), write(P), nl.


gera_populacao_schedule(Pop):-									
	populacao(TamPop),									
	% tarefas(NumT),				
	day(Dr),
	room(Rr),
	% buscar lista de cirurgias criada pelo Tiago (allocate_surgeries)
	findall(OpCode, operation_assigned_to_room(Rr, Dr, OpCode), ListaTarefas),
	length(ListaTarefas, NumT),
	assert(tarefas(NumT)),
	gera_populacao_schedule(TamPop,ListaTarefas,NumT,Pop).		
									
% criar os individuos (recursivo)								
gera_populacao_schedule(0,_,_,[]):-!.	
gera_populacao_schedule(TamPop,ListaTarefas,NumT,[Ind|Resto]):-	
	TamPop1 is TamPop-1,!,								
	%nl,write('Resto de gerapopulacao: '), write(ListaTarefas), nl,
	gera_populacao_schedule(TamPop1,ListaTarefas,NumT,Resto),!,
	gera_individuo_schedule(ListaTarefas,NumT,Ind),	
	% nl,write('Resto de gerapopulacao: '), write(Resto), nl,
	% verificar que individuo não existe
	not(member(Ind,Resto)).								
														
%gera_populacao_schedule(TamPop,ListaTarefas,NumT,L):-			
%	gera_populacao_schedule(TamPop,ListaTarefas,NumT,L).			
							
% Com base na lista de cirurgias alocadas à sala criar um individuo da populacao	
gera_individuo_schedule([],_,[]):-!.										
gera_individuo_schedule([G],1,[G]):-!.							
gera_individuo_schedule(ListaTarefas,NumT,[G|Resto]):-		
	NumTemp is NumT + 1, % To use with random	
	%write('gera_individuo_schedule: '), write(NumTemp), nl
	
	% escolher aleatoriament um elemento da lista de cirurgias para adicionar ao novo individuo
	%random_gen(1,NumTemp,N),							
	%random(1,NumTemp,N),
	randseq(1, NumT, [N|R]),
	%write('		gera_individuo_schedule: retira '), write(NumTemp),write(', lengthLista: '), length(ListaTarefas,Len), write(Len), write(', N: '), write(N), nl,
	%write('		gera_individuo_schedule:  ListaTarefas: '), write(ListaTarefas), nl,
	retira(N,ListaTarefas,G,NovaLista),!,	%write('		gera_individuo_schedule: retira~_next '),		nl,
	%permutation(ListaTarefas, [G|NovaLista]),
	NumT1 is NumT-1,								
	gera_individuo_schedule(NovaLista,NumT1,Resto).			
	

%random_gen(A,B,C):-
%	((random(A,B,C);random(A,B,C));random_gen(A,B,C)).

random_gen(A,A,A).
random_gen(A,B,C):-
	%write('		random_gen: '),write(' , '), write(A),write(' , '), write(B),nl,
	(
		(B=<A,!, C is A)
		;
		(random(A,B,C);random(A,B,C)) %; random_gen(A,B,C).
	).
	%((random(A,B,C);random(A,B,C)); random_gen(A,B,C)).

avalia_populacao_schedule(_,_,[],[]).																					
avalia_populacao_schedule(Room,Day,[Ind|Resto],[Ind*V|Resto1]):-																										
	avalia_schedule(Ind,Room,Day,V),																			
	avalia_populacao_schedule(Room,Day, Resto,Resto1).	

avalia_schedule(Seq,Room,Day,V):-
		retractall(agenda_staff1(_,_,_)),
		retractall(agenda_operation_room1(Room,_,_)),
		retractall(availability(_,_,_)),
		% findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
		% agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
		% findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),	
		findall(_,(agenda_staff2(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
		agenda_operation_room2(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
		findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),	
	(
		(
			% tenta agendar e usa a hora da ultima cirurgia como fitness
			availability_all_surgeries1(Seq, Room, Day),
			agenda_operation_room1(Room, Day, AgendaNova),!,
			reverse(AgendaNova, AgendaR),
			evaluate_final_time(AgendaR,Seq,Tfinal),
			V is 1441 - Tfinal
		)
		;
		(	
			% senao conseguir agendar o fitness é zero
			V is 0
		)
	).
	% ,nl,write('						Penalty='),write(V),nl.
	

gera_geracao_schedule(_,_,G,G,Pop):-!,										
	ordena_populacao(Pop,NPopOrd),
	retractall(latest_gen_indivuduals(_)),
	asserta(latest_gen_indivuduals(NPopOrd)).
	%asserta(latest_gen_indivuduals(NPopOrd)),
	%write('Geracao '), write(G), write(':'), nl, write(Pop), nl.
																
gera_geracao_schedule(Room,Day,N,G,Pop):-											
	%write('Geracao '), N2 is N+1 ,write(N2), write(':'), nl, 
	%write(Pop), nl,
	%write('cruzamento: '),
	cruzamento(Pop,NPop1),	
	%write('mutacao '),				
	mutacao(NPop1,NPop),	
	%write('avalia_populacao_schedule '),		!,					
	avalia_populacao_schedule(Room,Day,NPop,NPopAv),
	%ordena_populacao(NPopAv,NPopOrd),
	% A lista de individuos da geração é cruzada e mutada. Efetivamente o numeror de elementos é Duplicado. É preciso escolher os que passam para a geracao seguinte.
	append(Pop, NPopAv, NovaPop),
	% Escolher individuos que passam para a geracao seguinte.
	pop_nova_geracao(NovaPop, PopNova),
	%write('gera_geracao_schedule: '),length(PopNova, A), write('size:'),write(A), write(':'),write(PopNova), 
	N1 is N+1,			
	retractall(latest_gen_indivuduals(_)),
	ordena_populacao(PopNova,NPopOrd),
	asserta(latest_gen_indivuduals(NPopOrd)),
	gera_geracao_schedule(Room,Day,N1,G,PopNova).									
	
	
availability_all_surgeries3([],_,_).
availability_all_surgeries3([OpCode|LOpCode],Room,Day):-
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
    
	availability_all_surgeries3(LOpCode,Room,Day),!.
	




pop_nova_geracao(PopAntiga, Resultado):-
	% 1. elitismo elite(E)
	elite(E),
	populacao(NPop),
	NrElem1 is E/100,%write('pop_nova_geracao:NrElem1: '),write(NrElem1),nl,
	NrElem is floor(NrElem1 * NPop),%write('pop_nova_geracao:NrElem: '),write(NrElem),nl,
	ordena_populacao(PopAntiga, PopAntiga1),
	reverse(PopAntiga1, PopAntiga2),
	sublist(PopAntiga2, NrElem, Elite, RestoAntiga),
	%nl,write('pop_nova_geracao:Elite: '),write(NrElem),write(': '),length(Elite, SizeElite),write(SizeElite),nl, write(Elite),nl,
	%write(':'),write(Elite),nl,
	% 2. torneio
	torneio(RestoAntiga, Resultado_Torneio),
	%nl,write('Resultado_Torneio: '),length(Resultado_Torneio, SizeResultado_Torneio),write(SizeResultado_Torneio),nl,
%	write(':'),write(Resultado_Torneio),nl,
	% 3. ordenar e escolher 
	NrTorneiro is NPop - NrElem,
	sublist(Resultado_Torneio, NrTorneiro, B, Fim),
	append(Elite, B, Resultado).
	%append(Elite, Resultado_Torneio, Resultado).

sublist(List, N, Sublist, Rest) :-
    length(Sublist, N),       % Sublista tem tamanho N
    append(Sublist, Rest, List).  % Combina sublista e resto para reconstruir a lista.
	
	
torneio(Populacao, TorneioSelecionado):-
    torneio_seleciona(Populacao, TorneioSelecionado).

% torneio com base em fitness
torneio_seleciona([], _). 
torneio_seleciona([Last], []).
torneio_seleciona(Populacao, [Selecionado|RestoSelecionados]) :-
	% 
    select_aleatorio(Populacao, Ind1*V1, PopResto1),
    select_aleatorio(PopResto1, Ind2*V2, PopResto2),
    (
        V1 >= V2, Selecionado = Ind1*V1
    ;
        V2 > V1, Selecionado = Ind2*V2
    ),
    torneio_seleciona(PopResto2, RestoSelecionados).

select_aleatorio(Populacao, Ind*V, PopResto) :-
    random_permutation(Populacao, [Ind*V|PopResto]).