
/*
*   An exception to be thrown when a user is 
*   attempting to exit the source code
*/
public class ExitException : Exception{
    public int exitCode = 0;
    public ExitException(string msg, int exitCode = 0) : base( msg){
        this.exitCode = exitCode;
    }
}