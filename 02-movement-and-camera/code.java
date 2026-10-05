import java.util.*;
import java.io.*;

public class RobodogUnoptimised {
    public static void main(String[] args) throws IOException {
        Scanner sc = new Scanner(System.in);
        int t = sc.nextInt();
        
        while (t-- > 0) {
            int n = sc.nextInt();
            int k = sc.nextInt();
            int[] a = new int[n];
            
            for (int i = 0; i < n; i++) {
                a[i] = sc.nextInt();
            }
            
            HashMap<Integer, Integer> freqMap = new HashMap<>();
            for (int val : a) {
                freqMap.put(val, freqMap.getOrDefault(val, 0) + 1);
            }
            
            int d = freqMap.size();
            
            if (d == k) {
                System.out.println(0);
                continue;
            }
            
            if (d < k) {
                System.out.println(k - d);
                continue;
            }
            
            List<Integer> frequencies = new ArrayList<>(freqMap.values());
            Collections.sort(frequencies);
            
            int toRemove = d - k;
            int changes = 0;
            for (int i = 0; i < toRemove; i++) {
                changes += frequencies.get(i);
            }
            
            System.out.println(changes);
        }
    }
}
