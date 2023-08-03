module Todo.Util.Types

//create todo type
type Todo =
  { Id: System.Guid
    Title: string
    Content: string
    Completed: bool
    User_id: string }
