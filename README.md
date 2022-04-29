# Directory

Directory Api Quiz

## Requirements

The application should fulfill the following requirements:

- A member can be created using their name and a personal website address.
- When a member is created, all the heading (h1-h3) values are pulled in from the website to that
  members profile.
- The website url is shortened (e.g. using http://goo.gl).
- After the member has been added, I can define their friendships with other existing members.
  Friendships are bi-directional i.e. If David is a friend of Oliver, Oliver is always a friend of David as well.
- The interface should list all members with their name, short url and the number of friends.
- Viewing an actual member should display the name, website URL, shortening, website headings, and
  links to their friends' pages.
- Now, looking at Alan's profile, I want to find experts in the application who write about a certain topic and
  are not already friends of Alan.
- Results should show the path of introduction from Alan to the expert e.g. Alan wants to get introduced to
  someone who writes about 'Dog breeding'. Claudia's website has a heading tag "Dog breeding in
  Ukraine". Bart knows Alan and Claudia. An example search result would be Alan -> Bart -> Claudia ("Dog
  breeding in Ukraine").

## Solution

I decide to separate Microservices proyect and web project, It is more scalable and does not depend on a specific technology for UI (device, technology, etc)

## Techstack selection

- dotnet core so it could be multiplatform
- WebApi (c# as programing language) for the services
- EF core with InMemoryDatabase to persist the entities
- HtmlAgilityPack to handle html content
- Swashbuckle to autogenerate API documentation (also could be used for testing purposes)
- xunit to unit test the project
- Visual code as development environment
- Github as repository (it could be easily integrated with DevOps tools)
- Markdown for writing documentation

### Folder structure

- src
  - Controllers: Endpoints exposed to the user
  - Entitites: This are the models that are persisten in a database
  - Helpers: Meant for utilities, helpers, managers, etc
  - Models: The objects that receive or send data to the user
  - Repositories: Data access layer, mainly CRUDS
  - Services: Logic layer, this interacts between Controllers and Repositories

### Monitoring

There is a "ping" endpoint, this could be used to monitor the health of our service, we can easily check if service is running by calling it

There is a "throw" endpoint, this could be used to check the error handling. For development environment it is descriptive but for prod should not display details.

### API documentation

Is using swagger and it is configure to display the apis in a web interface that also could be used to test. It is configured to be displayed only in Development environments

### Logger

It is implemented and will help to track the user actions, this is also helful for application monitoring and troubleshooting errors

### Localization

The project is configured to use resource files for

### Test

Due to time constraints only few and basic test cases where created, even though, the unit test project is in place and also a Postman collection for manual test APIs (this could be automated with more time)

### Authentication

For authentication I decided to reuse code I wrote before, it is a oauth implementation also implemented in dotnet core and using EF for persisting users and tokens (not integrated yet)

## ToDo / Improvements

### Requirements

There are missing requirements that will be done in a second session, here are the missing requirements:

- Now, looking at Alan's profile, I want to find experts in the application who write about a certain topic and
  are not already friends of Alan.
- Results should show the path of introduction from Alan to the expert e.g. Alan wants to get introduced to
  someone who writes about 'Dog breeding'. Claudia's website has a heading tag "Dog breeding in
  Ukraine". Bart knows Alan and Claudia. An example search result would be Alan -> Bart -> Claudia ("Dog
  breeding in Ukraine").

### Authentication

It could be done by using oauth (jwt), authentication attributes will be added to the controller actions so this will add a security layer to the microservices

### CI/CD

Creating scripts to build and release the project, considering diferent environments, will speed up the go to production

### Repo Branching policies

Since this is a quiza, code is commited direcly in main branch, but will be better to have a branching strategy.

### Techdebt

- Add Automapper and DTOs, so it is better to cast entities to models
- Evaluate to replace complex logic in repositories with store procedures
- Create a db script for a relational database and configure the provider to be used when is not development environment
- Add more unit testing
- Make the scrapper async and maybe add frequency to keep the headings updated
