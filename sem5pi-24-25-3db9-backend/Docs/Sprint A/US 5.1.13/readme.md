# US 5.1.13

## 1. Context

This task appears in the start of the project's development, to be able to edit a staff profile.


## 2. Requirements

**US 5.1.13** As an Admin, I want to edit a staff's profile, so that I can update their information.

**Acceptance Criteria:**

- Admins can search for and select a staff profile to edit.
- Editable fields include contact information, availability slots, and specialization.
- The system logs all profile changes, and any changes to contact information trigger a confirmation email to the staff member.
- The edited data is updated in real-time across the system.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin and that the staff profile is already in the system.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

*"Can the same user have both a patient and a healthcare profile? No..."*


*"When it comes to patients and healthcare staff ... So the email is the identifying attribute, right? For users, or is it the username? It's the username, okay? But typically, as you know, nowadays, most of the usernames that you have in all the systems are your email, okay? So they hack, kind of, instead of you allowing to create a specific username, you use your own email as the username ... you should use the email as the username."*

- **Question:** How should the specialization be assigned to a staff? Should the admin write it like a first name? Or should the admin select the specialization?
  - **Answer:** The system has a list of specializations. staff is assigned a specialization from that list.


- **Question:** Boa tarde, gostaria de saber se é objetivo o sistema diferenciar as especializações para cada tipo de staff. Ou seja se temos de validar que a especialização x só pode ser atribuída por exemplo a um membro do staff que seja doctor, ou se consideramos que qualquer especialização existente no sistema pode ser atribuída a qualquer staff ficando da autoria do responsável por criar os perfis atribuir especializações válidas de acordo com a role do staff.
  - **Answer:** As especializações são independentes do professional ser médico ou enfermeiro


- **Question:** -Médicos e enfermeiros podem ter apenas uma especialidade ou podem ser especialistas em várias? -Quem faz parte do staff? Toda a gente na sala de operação? Se sim, todos eles tem as suas respetivas especialidades, incluindo técnicos? 
  - **Answer:** Um médico ou enfermeiro apenas tem uma especialização. no staff apenas consideramos médicos e enfermeiros


- **Question:** Regarding the specializations, do doctors, nurses, and technicians share the same group of specializations, or does each type of professional have distinct, role-specific specializations? Could you clarify how these specializations are categorized?
  - **Answer:** They share the same set of specializations.


*"When we are registering users to the platform, do we immediately need to give them a profile? Like when we register a user, do we immediately need to create their patient or healthcare professional staff profile? Or can we just leave them as a user to log in? Because there's some user stories that reference the creation of users with roles, but not necessarily their profile. I think that's up to you, honestly.
It will boil down to a design decision. From the functional perspective, it's not something important. Okay, so I think it will be more a question of does it make more sense for you from the technical perspective to do it immediately or do it afterwards?"*

- **Question:** Are healthcare staff IDs unique across roles?
  - **Answer:** Yes, staff IDs are unique and not role-specific (e.g., a doctor and nurse can share the same ID format).

*"... regarding healthcare staff, we want to understand if the staff ID is unique or if it is, for example, if it is unique in the sense that, for example, doctor is 1, nurse is 2, or if, for example, the doctor has ID 1, there is a nurse ID 1? Okay, I understand what the issue is. Employees are identified by a mechanical number, basically. And it doesn't matter if this typing number is for a doctor, a nurse, or an assistant. It's a number of employees ..."*


- **Question:** Can you clarify the difference between mechanographic number, staff id and license number?
  - **Answer:** The staff id and mechanographic number is the same concept. the license number is the number assigned by the professional guild (ex., "ordem dos enfermeiros", "ordem dos médicos") to the doctor or nurse attesting they legally can perform the medical acts of their profession.


- **Question:** I have one question related to the staff license number. Since it will be generated, would you like it to be generated in any particular format or algorithm of your choice?
  - **Answer:** There is a misinformation in the RFP. staff id are unique and generated by the system. License numbers are unique but are not generated by the system. Staff id follow the format "(N | D | O)yyyynnnnn", for instance, N202401234, N is for nurse, D is for doctor, O is for other. yyyy is the year of recruitment. nnnnn is a sequential number. License numbers are assigned by the professional guild. the admin will enter the license number and the system records it
  

- **Question:** There are 2 separate use cases regarding backoffice users: One for the creation of the user account and another one for the creation of the staff's profile. Is there a fixed order for these operations to take place? Does the admin always create the profile first or can he create the user first aswell? If the profile is created first, for example, should the user be created automaticaly or should the admin create the user afterwards, having to do 2 distinct operations?
  - **Answer:** Recommended Flow: Order of operations: The system should support profile first. The admin should then create the user account. the account and user profile are linked by the professional email address or username (depending on the IAM provider).
 Distinct Operations: The operations should remain distinct, even if they are performed in quick succession. This ensures that each step (creating user credentials and creating a staff profile) is carefully tracked and managed. Validation: The system should ensure that a staff profile and user account are both created and linked before the staff member can access the system. 
  

- **Question:** Can you clarify the username and email requirements?
  - **Answer:** The username is the "official" email address of the user. for backoffice users, this is the mechanographic number of the collaborator, e.g., D240003 or N190345, and the DNS domain of the system. For instance, Doctor Manuela Fernandes has email "D180023@myhospital.com". The system must allow for an easy configuration of the DNS domain (e.g., environment variable). For patients, the username is the email address provided in the patient record and used as identity in the external IAM. for instance patient Carlos Silva has provided his email csilva98@gmail.com the first time he entered the hospital. That email address will be his username when he self-registers in the system


**Observations:**

Only the staff phone number can be edited, since the email is generated automatically by the system upon profile creation.

The confirmation email after updating the staff phone number will be sent by default to an email configured in the appsettings.json file, since the staff's email doesn't exist in the accademic context of this project. In future releases, to actually send the email to the staff's real email, the email in the config file can be deleted and the code will adapt to the change.

### 3.1. Domain Model excerpt
![DM Staff](DM%20Staff.png)


### 3.2. HTTP requests

The following **HTTP requests** will be implemented:
- GET (to check available staff member)
- PUT (to edit a specific staff member)
- PATCH (to edit some data of a specific staff member)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US5.1.13 N1 SD](US5.1.13%20N1%20SD.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US5.1.13 N2 SD](US5.1.13%20N2%20SD.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

SD for the PUT request:

![US5.1.13 N3 SD PUT](US5.1.13%20N3%20SD%20PUT.svg)

SD for the PATCH request:

![US5.1.13 N3 SD PATCH](US5.1.13%20N3%20SD%20PATCH.svg)


### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

The following tests are to be developed:
- updating an existing staff with new data
- updating an existing staff with some new data, like phone number, specialization and availability slots~
  - if the new data is invalid, exceptions should be thrown

All Value Objects will be tested in Unitary Tests, to check if they fullfill their requirements.

The Staff Service will be tested to see if the updated Staff is correct.

The Staff Controller will be tested to see if the updated Staff and responses are correct.

Postman Tests will also check the updated Staff data and the responses, for both success and failure cases.



## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #16 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To edit a staff profile, run the Backoffice app and send a PUT or PATCH HTTP request with the staff ID and the new staff data.
Then send a GET request with the staff ID and check the updated data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.