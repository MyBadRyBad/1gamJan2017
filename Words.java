import java.util.ArrayList;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import javax.net.ssl.HttpsURLConnection;

public class Words {

    private final String USER_AGENT = "Mozilla/5.0";

    public static void main(String[] args) throws Exception {

        if (args.length != 1) {
            System.out.println("usage: java Words textfile.txt");
        } else {
            Words words = new Words();
            words.requestRelatedWords();
        }
    }
    
    private ArrayList<String> readFile(String filename) throws Exception {
         FileInputStream fstream = new FileInputStream(filename);
         BufferedReader br = new BufferedReader(new InputStreamReader(fstream));
            
        String strLine;

        while ((strLine = br.readLine()) != null) {
            System.out.println(strLine);
        }

        br.close();

        return new ArrayList<String>();
    }

    private void requestRelatedWords() throws Exception {
        String url = "http://pydictionary-geekpradd.rhcloud.com/api/meaning/jungle";

        URL obj = new URL(url);
        HttpURLConnection con = (HttpURLConnection) obj.openConnection();

        // optional default is GET
        con.setRequestMethod("GET");

        //add request header
        con.setRequestProperty("User-Agent", USER_AGENT);

        int responseCode = con.getResponseCode();
        System.out.println("\nSending 'GET' request to URL : " + url);
        System.out.println("Response Code : " + responseCode);

        BufferedReader in = new BufferedReader(
                new InputStreamReader(con.getInputStream()));
        String inputLine;
        StringBuffer response = new StringBuffer();

        while ((inputLine = in.readLine()) != null) {
            response.append(inputLine);
        }
        in.close();

        //print result
        System.out.println(response.toString());
    }

}

