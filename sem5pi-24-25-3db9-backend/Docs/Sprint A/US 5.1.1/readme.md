# US 5.1.1

## 1. Context

This task appears in the start of the project's development, to be able to register new backoffice users.


## 2. Requirements

**US 5.1.1** As an Admin, I want to register new backoffice users (e.g., doctors, nurses, technicians, admins) via an out-of-band process, so that they can access the backoffice system with appropriate permissions.

**Acceptance Criteria:**

- Backoffice users (e.g., doctors, nurses, technicians) are registered by an Admin via an internal process, not via self-registration.
- Admin assigns roles (e.g., Doctor, Nurse, Technician) during the registration process.
- Registered users receive a one-time setup link via email to set their password and activate their account.
- The system enforces strong password requirements for security.
- A confirmation email is sent to verify the user’s registration.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- *"When it comes to patients and healthcare staff ... So the email is the identifying attribute, right? For users, or is it the username? It's the username, okay? But typically, as you know, nowadays, most of the usernames that you have in all the systems are your email, okay? So they hack, kind of, instead of you allowing to create a specific username, you use your own email as the username ... you should use the email as the username."*

- **Question:** Chapter 3.2 says that "Backoffice users are registered by the admin in the IAM through an out-of-band process.", but US 5.1.1 says that "Backoffice users are registered by an Admin via an internal process, not via self-registration.". Can you please clarify if backoffice users registration uses the IAM system? And if the IAM system is the out-of-band process?
  - **Answer:** what this means is that backoffice users can not self-register in the system like the patients do. the admin must register the backoffice user. If you are using an external IAM (e.g., Google, Azzure, Linkedin, ...) the backoffice user must first create their account in the IAM provider and then pass the credential info to the admin so that the user account in the system is "linked" wit the external identity provider.


- **Question:** Can you clarify the username and email requirements?
  - **Answer:** the username is the "official" email address of the user. for backoffice users, this is the mechanographic number of the collaborator, e.g., D240003 or N190345, and the DNS domain of the system. For instance, Doctor Manuela Fernandes has email "D180023@myhospital.com". The system must allow for an easy configuration of the DNS domain (e.g., environment variable).
For patients, the username is the email address provided in the patient record and used as identity in the external IAM. for instance patient Carlos Silva has provided his email csilva98@gmail.com the first time he entered the hospital. That email address will be his username when he self-registers in the system.


- **Question:** In a previous answer you said: "If you are using an external IAM (e.g., Google, Azzure, Linkedin, ...) the backoffice user must first create their account in the IAM provider and then pass the credential info to the admin so that the user account in the system is "linked" with the external identity provider." However, in the acceptance criteria it says "- Backoffice users (e.g., doctors, nurses, technicians) are registered by an Admin via an internal process, not via self-registration.". So should the backoffice user first be registered (by himself) in the IAM and then pass the info (email and password?) to the admin so that he creates his account? Or should he only register himself in the IAM after the admin creates his account in the system and providing the staff user his hospital's email? Can you clarify a little bit more what do you want to see as the process flow? In another answer you said: "the username is the "official" email address of the user. for backoffice users, this is the mechanographic number of the collaborator, e.g., D240003 or N190345, and the DNS domain of the system. For instance, Doctor Manuela Fernandes has email "D180023@myhospital.com"." If we use a DNS domain like the one above and, so, those emails do not actually exist, how can staff receive the confirmation link to activate their account?
  - **Answer:** In that same clarification https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=31510#p39978 you may find that "The system must allow for an easy configuration of the DNS domain (e.g., environment variable)."


- **Question:** What are the system's password requirements?
  - **Answer:** at least 10 characters long, at least a digit, a capital letter and a special character


The following **HTTP requests** will be implemented:
- POST (to register the new user)
- GET (to check the new users)
- GET by ID (to find the new user)
- PATCH (updating the password and active the account)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US5.1.1 N1 SD](US5.1.1%20N1%20SD.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US5.1.1 N2 SD](US5.1.1%20N2%20SD.svg)


### 4.3. Level 3 Sequence Diagram

These diagrams guide the realization of the functionality, for level 3 process view.
For organizational reasons this level was separated in two parts, each with a different actor and their respective actions.

Administrator :

![US5.1.1 N3 SD](US5.1.1%20N3%20SD.svg)

Backoffice user:

![US5.1.1 N3 SD User](US5.1.1%20N3%20SD%20User.svg)


### 4.4. Class Diagram

This diagram presents the classes that support the functionality.
*To do*


### 4.5. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.6. Tests

The following tests are to be developed:
- The password must have at least 10 characters long.
- The password must have at least a digit.
- The password must have at least a capital letter.
- The password must have at least a special character.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using the #1 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new user, run the Backoffice app and send a POST HTTP request with the new user data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.